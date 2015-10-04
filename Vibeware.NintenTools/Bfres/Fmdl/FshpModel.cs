namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents an FSHP subsection in a FMDL section which describes how data in a vertex buffer is drawn.
    /// </summary>
    [DebuggerDisplay("Model {Name}")]
    public class FshpModel
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FshpModel"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FshpModel(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FshpModel"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name to describe the resulting polygon of a model.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the material used to draw this part of the model.
        /// </summary>
        public FmatMaterial Material
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bone to which the vertices of this part of the model are attached for animations.
        /// </summary>
        public FsklBone Bone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="FvtxVertexBuffer"/> instance referenced to draw the vertex data from.
        /// </summary>
        public FvtxVertexBuffer VertexBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FshpLodModel"/> instances describing the polygons appearance at
        /// different distances.
        /// </summary>
        public List<FshpLodModel> LodModels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the visibility tree nodes.
        /// </summary>
        public List<FshpVisibilityNode> VisibilityNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of visibility indices which seem to reference the leaf nodes of the visibility node
        /// tree.
        /// </summary>
        public List<ushort> VisibilityIndices
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of FSKL skeleton offsets. They seem to state the index of a bone to which a part of the
        /// model should be positioned upon load.
        /// Almost all Mario Kart 8 character models reference the index of the "Head" bone to position the "Pupil"
        /// model there. If this is ignored, the "Pupil" mesh will be positioned to the scene origin, near the legs of
        /// the model.
        /// </summary>
        public List<ushort> SkeletonOffsets
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();
            
            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FSHP")
            {
                throw new BfresException("FshpModel identifier invalid");
            }

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x08 != 2)
            {
                context.Warnings.Add("FshpModel.Unknown0x08 has unexpected value of " + Internal.Unknown0x08);
            }

            Internal.Index = context.Reader.ReadUInt16();
            Internal.MaterialIndex = context.Reader.ReadUInt16();
            Internal.BoneIndex = context.Reader.ReadUInt16();
            Internal.VertexBufferIndex = context.Reader.ReadUInt16();
            Internal.SkeletonIndexArrayCount = context.Reader.ReadUInt16();
            Internal.Unknown0x16 = context.Reader.ReadByte();
            Internal.LodModelCount = context.Reader.ReadByte();
            Internal.VisibilityNodeCount = context.Reader.ReadUInt32();
            Internal.Unknown0x1C = context.Reader.ReadSingle();
            Internal.VertexBufferOffset = context.Reader.ReadBfresOffset();
            Internal.LodModelOffset = context.Reader.ReadBfresOffset();
            Internal.SkeletonIndexArrayOffset = context.Reader.ReadBfresOffset();

            Internal.Unknown0x2C = context.Reader.ReadUInt32();
            if (Internal.Unknown0x2C != 0)
            {
                context.Warnings.Add("FshpModel.Unknown0x2C has unexpected value of " + Internal.Unknown0x2C);
            }

            Internal.VisibilityNodesOffset = context.Reader.ReadBfresOffset();
            Internal.VisibilityRangesOffset = context.Reader.ReadBfresOffset();
            Internal.VisibilityIndicesOffset = context.Reader.ReadBfresOffset();

            Internal.Unknown0x3C = context.Reader.ReadUInt32();
            if (Internal.Unknown0x3C != 0)
            {
                context.Warnings.Add("FshpModel.Unknown0x3C has unexpected value of " + Internal.Unknown0x3C);
            }

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadLodModels(context);
                LoadVisibilityNodes(context);
                LoadVisibilityIndices(context);
                LoadSkeletonOffsets(context);
            }
        }

        private void LoadLodModels(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.LodModelOffset.ToFile;
            LodModels = new List<FshpLodModel>(Internal.LodModelCount);
            for (int i = 0; i < Internal.LodModelCount; i++)
            {
                LodModels.Add(new FshpLodModel(context));
            }
        }

        private void LoadVisibilityNodes(BfresLoaderContext context)
        {
            // Load the raw node instances.
            context.Reader.Position = Internal.VisibilityNodesOffset.ToFile;
            VisibilityNodes = new List<FshpVisibilityNode>((int)Internal.VisibilityNodeCount);
            for (int i = 0; i < Internal.VisibilityNodeCount; i++)
            {
                VisibilityNodes.Add(new FshpVisibilityNode(context));
            }

            // Rebuild the tree structure, visibility group slices and map the ranges for the nodes.
            context.Reader.Position = Internal.VisibilityRangesOffset.ToFile;
            for (int i = 0; i < VisibilityNodes.Count; i++)
            {
                FshpVisibilityNode node = VisibilityNodes[i];

                // Map the left and right child nodes in the binary tree structure.
                if (node.Internal.LeftNodeIndex != i)
                {
                    node.LeftNode = VisibilityNodes[node.Internal.LeftNodeIndex];
                }
                if (node.Internal.RightNodeIndex != i)
                {
                    node.RightNode = VisibilityNodes[node.Internal.RightNodeIndex];
                }


                // Add the slice of visibility groups as instances to the node's list.
                node.VisibilityGroups = new List<FshpVisibilityGroup>(node.Internal.VisibilityGroupCount);
                for (int j = 0; j < node.Internal.VisibilityGroupCount; j++)
                {
                    node.VisibilityGroups.Add(LodModels[0].VisibilityGroups[j + node.Internal.VisibilityGroupIndex]);
                }

                // Read in the ranges and add them to the node.
                node.UnknownPosition1 = context.Reader.ReadVector3F();
                node.UnknownPosition2 = context.Reader.ReadVector3F();
            }
        }

        private void LoadVisibilityIndices(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.VisibilityIndicesOffset.ToFile;
            VisibilityIndices = new List<ushort>(context.Reader.ReadUInt16s((int)Internal.VisibilityNodeCount));
        }

        private void LoadSkeletonOffsets(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.SkeletonIndexArrayOffset.ToFile;
            SkeletonOffsets = new List<ushort>(context.Reader.ReadUInt16s(Internal.SkeletonIndexArrayCount));
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FshpModel"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the file name without the extension in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 2.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of this model in the parent FMDL sections model array..
            /// </summary>
            public ushort Index
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the <see cref="FmatMaterial"/> to reference.
            /// </summary>
            public ushort MaterialIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the <see cref="FsklBone"/> to which this model is attached to.
            /// </summary>
            public ushort BoneIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the <see cref="FvtxVertexData"/> from which vertex data is referenced.
            /// </summary>
            public ushort VertexBufferIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an index dealing with the skeleton of the model, but has an unknown purpose and is often 0.
            /// </summary>
            public ushort SkeletonIndexArrayCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x16. The value seems to be non-zero if
            /// <see cref="SkeletonIndexArrayCount"/> is set, otherwise 0.
            /// </summary>
            public byte Unknown0x16
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FshpLodModel"/> instances in the <see cref="LodModels"/> list.
            /// </summary>
            public byte LodModelCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of<see cref="FshpVisibilityNode"/> instances in the <see cref="VisibilityNodes"/>
            /// hierarchy.
            /// </summary>
            public uint VisibilityNodeCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x1C.
            /// </summary>
            public float Unknown0x1C
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the offset to the referenced <see cref="FvtxVertexData"/>.
            /// </summary>
            public BfresOffset VertexBufferOffset
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the offset to the first <see cref="FshpLodModel"/> instance.
            /// </summary>
            public BfresOffset LodModelOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="ushort"/> in the skeleton index array with unknown
            /// purpose.
            /// </summary>
            public BfresOffset SkeletonIndexArrayOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x2C. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x2C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FshpVisibilityNode"/> instance.
            /// </summary>
            public BfresOffset VisibilityNodesOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FshpVisibilityRange"/> instance.
            /// </summary>
            public BfresOffset VisibilityRangesOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FshpVisibilityIndex"/> instance.
            /// </summary>
            public BfresOffset VisibilityIndicesOffset
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets an unknown variable at offset 0x3C. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x3C
            {
                get;
                set;
            }
        }
    }
}
namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents an FMDL section in a BFRES file which contains model data.
    /// </summary>
    [DebuggerDisplay("FMDL {Name}")]
    public class FmdlSection
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmdlSection"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmdlSection(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmdlSection"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of this FMDL section.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the skeleton stored for the represented model to allow animations with it.
        /// </summary>
        public FsklSkeleton Skeleton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of vertex buffers describing the vertex data for each polygon.
        /// </summary>
        public List<FvtxVertexBuffer> VertexBuffers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of models describing how the data of the vertex buffer has to be drawn.
        /// </summary>
        public List<FshpModel> Models
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of materials describing how surfaces of polygons look.
        /// </summary>
        public List<FmatMaterial> Materials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of generic parameters describing additional information about the model.
        /// </summary>
        public List<FmdlParameter> Parameters
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FMDL")
            {
                throw new BfresException("FmdlSection identifier invalid");
            }

            // Read section properties.
            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            Internal.EndOfBfresStringTable = context.Reader.ReadBfresOffset();

            // Read subsection offsets.
            Internal.SkeletonOffset = context.Reader.ReadBfresOffset();
            Internal.VertexBufferOffset = context.Reader.ReadBfresOffset();
            Internal.ModelsIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.MaterialsIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.ParameterIndexGroupOffset = context.Reader.ReadBfresOffset();

            // Read subsection counts.
            Internal.VertexBufferCount = context.Reader.ReadUInt16();
            Internal.ModelCount = context.Reader.ReadUInt16();
            Internal.MaterialCount = context.Reader.ReadUInt16();
            Internal.ParameterCount = context.Reader.ReadUInt16();
            Internal.Unknown0x28 = context.Reader.ReadUInt32();

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                // Order matters for LoadModels(), as it maps existing materials, bones and vertex buffers.
                LoadSkeleton(context);
                LoadVertexBuffers(context);
                LoadMaterials(context);
                LoadModels(context);
                LoadParameters(context);
            }
        }
        
        private void LoadSkeleton(BfresLoaderContext context)
        {
            if (!Internal.SkeletonOffset.IsEmpty)
            {
                context.Reader.Position = Internal.SkeletonOffset.ToFile;
                Skeleton = new FsklSkeleton(context);
            }
        }

        private void LoadVertexBuffers(BfresLoaderContext context)
        {
            if (!Internal.VertexBufferOffset.IsEmpty)
            {
                context.Reader.Position = Internal.VertexBufferOffset.ToFile;
                VertexBuffers = new List<FvtxVertexBuffer>(Internal.VertexBufferCount);
                for (int i = 0; i < Internal.VertexBufferCount; i++)
                {
                    VertexBuffers.Add(new FvtxVertexBuffer(context));
                }
            }
        }

        private void LoadModels(BfresLoaderContext context)
        {
            if (!Internal.ModelsIndexGroupOffset.IsEmpty)
            {
                context.Reader.Position = Internal.ModelsIndexGroupOffset.ToFile;
                Internal.ModelsIndexGroup = new BfresIndexGroup(context);
                if (Internal.ModelCount != Internal.ModelsIndexGroup.Nodes.Length - 1)
                {
                    context.Warnings.Add("FmdlSection.ModelCount does not match count of index group.");
                }

                Models = new List<FshpModel>(Internal.ModelsIndexGroup.Nodes.Length - 1);
                for (int i = 1; i < Internal.ModelsIndexGroup.Nodes.Length; i++)
                {
                    BfresIndexGroupNode node = Internal.ModelsIndexGroup.Nodes[i];
                    context.Reader.Position = node.DataPointer.ToFile;
                    Models.Add(new FshpModel(context));
                }
            }
        }

        private void LoadMaterials(BfresLoaderContext context)
        {
            if (!Internal.MaterialsIndexGroupOffset.IsEmpty)
            {
                context.Reader.Position = Internal.MaterialsIndexGroupOffset.ToFile;
                Internal.MaterialsIndexGroup = new BfresIndexGroup(context);
                if (Internal.MaterialCount != Internal.MaterialsIndexGroup.Nodes.Length - 1)
                {
                    context.Warnings.Add("FmdlSection.MaterialCount does not match count of index group.");
                }

                Materials = new List<FmatMaterial>(Internal.MaterialsIndexGroup.Nodes.Length - 1);
                for (int i = 1; i < Internal.MaterialsIndexGroup.Nodes.Length; i++)
                {
                    BfresIndexGroupNode node = Internal.MaterialsIndexGroup.Nodes[i];
                    context.Reader.Position = node.DataPointer.ToFile;
                    Materials.Add(new FmatMaterial(context));
                }
            }
        }

        private void LoadParameters(BfresLoaderContext context)
        {
            // Read parameter subsections.
            if (!Internal.ParameterIndexGroupOffset.IsEmpty)
            {
                context.Reader.Position = Internal.ParameterIndexGroupOffset.ToFile;
                Internal.ParameterIndexGroup = new BfresIndexGroup(context);
                if (Internal.ParameterCount != Internal.ParameterIndexGroup.Nodes.Length - 1)
                {
                    context.Warnings.Add("FmdlSection.ParameterCount does not match count of index group.");
                }

                Parameters = new List<FmdlParameter>(Internal.ParameterIndexGroup.Nodes.Length - 1);
                for (int i = 1; i < Internal.ParameterIndexGroup.Nodes.Length; i++)
                {
                    BfresIndexGroupNode node = Internal.ParameterIndexGroup.Nodes[i];
                    context.Reader.Position = node.DataPointer.ToFile;
                    Parameters.Add(new FmdlParameter(context));
                }
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmdlSection"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the name in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the end of the string table in the <see cref="BfresFile"/> instance.
            /// </summary>
            public BfresOffset EndOfBfresStringTable
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="FsklSkeleton"/> instance.
            /// </summary>
            public BfresOffset SkeletonOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FvtxVertexBuffer"/> instance.
            /// </summary>
            public BfresOffset VertexBufferOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing <see cref="FshpModel"/>
            /// instances.
            /// </summary>
            public BfresOffset ModelsIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing instances in the <see cref="Models"/> list.
            /// </summary>
            public BfresIndexGroup ModelsIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing <see cref="FmatMaterial"/>
            /// instances.
            /// </summary>
            public BfresOffset MaterialsIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing instances in the <see cref="Materials"/> list.
            /// </summary>
            public BfresIndexGroup MaterialsIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing <see cref="FmdlParameter"/>
            /// instances.
            /// </summary>
            public BfresOffset ParameterIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing instances in the <see cref="Parameters"/> list.
            /// </summary>
            public BfresIndexGroup ParameterIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FvtxVertexBuffer"/> instances in the <see cref="VertexBuffers"/> list.
            /// </summary>
            public ushort VertexBufferCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FshpModel"/> instances in the <see cref="Models"/> list.
            /// </summary>
            public ushort ModelCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmatMaterial"/> instances in the <see cref="Materials"/> list.
            /// </summary>
            public ushort MaterialCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmdlParameter"/> instances in the <see cref="Parameters"/> list.
            /// </summary>
            public ushort ParameterCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x28. The value seems to be an unused face count.
            /// </summary>
            public uint Unknown0x28
            {
                get;
                set;
            }
        }
    }
}
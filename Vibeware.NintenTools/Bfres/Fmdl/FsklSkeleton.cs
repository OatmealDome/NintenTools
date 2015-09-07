namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using Vibeware.NintenTools.IO;
    using Vibeware.NintenTools.Maths;

    /// <summary>
    /// Represents an FSKL subsection in a FMDL section which describes a models armature consisting out of bones.
    /// </summary>
    public class FsklSkeleton
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FsklSkeleton"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FsklSkeleton(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FsklSkeleton"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FsklBone"/> instances representing the armatures structure.
        /// </summary>
        public List<FsklBone> Bones
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FSKL")
            {
                throw new BfresException("FsklSkeleton identifier invalid");
            }

            Internal.Unknown0x04 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x04 != 0)
            {
                context.Warnings.Add("FsklSkeleton.Unknown0x04 has unexpected value of " + Internal.Unknown0x04);
            }
            Internal.Unknown0x06 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x06 != 0x1100 && Internal.Unknown0x06 != 0x1200)
            {
                context.Warnings.Add("FsklSkeleton.Unknown0x06 has unexpected value of " + Internal.Unknown0x06);
            }

            Internal.BoneCount = context.Reader.ReadUInt16();
            Internal.InverseIndexCount = context.Reader.ReadUInt16();
            Internal.ExtraIndexCount = context.Reader.ReadUInt16();
            
            Internal.Unknown0x0E = context.Reader.ReadUInt16();
            if (Internal.Unknown0x0E != 0)
            {
                context.Warnings.Add("FsklSkeleton.Unknown0x0E has unexpected value of " + Internal.Unknown0x0E);
            }

            Internal.BoneIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.BoneArrayOffset = context.Reader.ReadBfresOffset();
            Internal.InverseIndexArrayOffset = context.Reader.ReadBfresOffset();
            Internal.InverseMatrixArrayOffset = context.Reader.ReadBfresOffset();

            if (context.Reader.ReadUInt32() != 0)
            {
                context.Warnings.Add("FsklSkeleton padding not empty");
            }

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadBones(context);
            }
        }

        private void LoadBones(BfresLoaderContext context)
        {
            // Load the index group.
            if (!Internal.BoneIndexGroupOffset.IsEmpty)
            {
                context.Reader.Position = Internal.BoneIndexGroupOffset.ToFile;
                Internal.BoneIndexGroup = new BfresIndexGroup(context);
                if (Internal.BoneCount != Internal.BoneIndexGroup.Nodes.Length - 1)
                {
                    context.Warnings.Add("FsklSkeleton.BoneCount does not match count of index group.");
                }
            }

            // Load the bone array. We do not use the index group as the order of the array indices matter.
            if (!Internal.BoneArrayOffset.IsEmpty)
            {
                context.Reader.Position = Internal.BoneArrayOffset.ToFile;
                Bones = new List<FsklBone>(Internal.BoneCount);
                for (int i = 0; i < Internal.BoneCount; i++)
                {
                    Bones.Add(new FsklBone(context));
                }
            }

            // Load the inverse indices.
            if (!Internal.InverseIndexArrayOffset.IsEmpty)
            {
                context.Reader.Position = Internal.InverseIndexArrayOffset.ToFile;
                Internal.InverseIndexArray = context.Reader.ReadUInt16s(Internal.InverseIndexCount);
                Internal.ExtraIndexArray = context.Reader.ReadUInt16s(Internal.ExtraIndexCount);
            }

            // Load the inverse matrices.
            if (!Internal.InverseMatrixArrayOffset.IsEmpty)
            {
                context.Reader.Position = Internal.InverseMatrixArrayOffset.ToFile;
                Internal.InverseMatrixArray = new Matrix4x3[Internal.InverseIndexCount];
                for (int i = 0; i < Internal.InverseMatrixArray.Length; i++)
                {
                    Internal.InverseMatrixArray[i] = context.Reader.ReadMatrix4x3();
                }
            }

            // Map the inverse matrices to the bones.
            for (int i = 0; i < Internal.InverseIndexArray.Length; i++)
            {
                Bones[Internal.InverseIndexArray[i]].InverseMatrix = Internal.InverseMatrixArray[i];
            }

            // Map the parent bones to the child bones.
            foreach (FsklBone bone in Bones)
            {
                bone.Parents = new FsklBone[bone.Internal.ParentIndices.Length];
                for (int i = 0; i < bone.Internal.ParentIndices.Length; i++)
                {
                    ushort parentIndex = bone.Internal.ParentIndices[i];
                    if (parentIndex != 0xFFFF)
                    {
                        bone.Parents[i] = Bones[parentIndex];
                    }
                }
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FsklSkeleton"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be 0x1100 or 0x1200.
            /// </summary>
            public ushort Unknown0x06
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of bones in the <see cref="Bones"/> list.
            /// </summary>
            public ushort BoneCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of indices mapping bone indices to inverse matrices.
            /// </summary>
            public ushort InverseIndexCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of additional indices at the end of the inverse index array, which have unknown
            /// purpose.
            /// </summary>
            public ushort ExtraIndexCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x0E
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing <see cref="FsklBone"/>
            /// instances.
            /// </summary>
            public BfresOffset BoneIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing instances in the <see cref="Bones"/> list.
            /// </summary>
            public BfresIndexGroup BoneIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FsklBone"/> instance.
            /// </summary>
            public BfresOffset BoneArrayOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first index in the bone index to inverse matrix mapping array.
            /// </summary>
            public BfresOffset InverseIndexArrayOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the array of inverse indices referring to bones having an inverse matrix.
            /// </summary>
            public ushort[] InverseIndexArray
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first inverse matrix.
            /// </summary>
            public BfresOffset InverseMatrixArrayOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the array of inverse matrices mapped to a bone through the indices of the
            /// <see cref="InverseIndexArray"/>.
            /// </summary>
            public Matrix4x3[] InverseMatrixArray
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the extra indices which have not been mapped to <see cref="FsklBone"/> instances in the
            /// <see cref="Bones"/> list.
            /// </summary>
            public ushort[] ExtraIndexArray
            {
                get;
                set;
            }
        }
    }
}
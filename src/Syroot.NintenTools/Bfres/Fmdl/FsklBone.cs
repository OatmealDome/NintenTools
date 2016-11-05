using System.Diagnostics;
using Syroot.NintenTools.IO;
using Syroot.NintenTools.Maths;

namespace Syroot.NintenTools.Bfres.Fmdl
{
    /// <summary>
    /// Represents a single bone of a <see cref="FsklSkeleton"/>.
    /// </summary>
    [DebuggerDisplay("Bone {Name}")]
    public class FsklBone
    {
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FsklBone"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an offset to the bones name.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the bone in the parents <see cref="FsklSkeleton"/> bone array.
            /// </summary>
            public ushort Index
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the indices of parent bones. A value is 0xFFFF if there is no parent at that position.
            /// </summary>
            public ushort[] ParentIndices
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0E. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x0E
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a flag set with unknown meaning.
            /// </summary>
            public ushort Flags
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x12. The value always seems to be 0x1001.
            /// </summary>
            public ushort Unknown0x12
            {
                get;
                set;
            }
        }

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FsklBone"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FsklBone(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FsklBone"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of this bone, generally describing the body purpose it represents.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an array of up to four <see cref="FsklBone"/> instances which are this bone's parents.
        /// </summary>
        public FsklBone[] Parents
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scaling of this bone.
        /// </summary>
        public Vector3F Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rotation of this bone. Probably a quaternion with the W component always being 1.0f.
        /// </summary>
        public Vector4F Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position of this bone.
        /// </summary>
        public Vector3F Translation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a transformation matrix reversing this and all parents bones transformations.
        /// </summary>
        public Matrix4x3 InverseMatrix
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();
            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            Internal.Index = context.Reader.ReadUInt16();
            Internal.ParentIndices = context.Reader.ReadUInt16s(4);
            Internal.Unknown0x0E = context.Reader.ReadUInt16();
            Internal.Flags = context.Reader.ReadUInt16();
            Internal.Unknown0x12 = context.Reader.ReadUInt16();
            Scale = context.Reader.ReadVector3F();
            Rotation = context.Reader.ReadVector4F();
            Translation = context.Reader.ReadVector3F();

            if (context.Reader.ReadUInt32() != 0)
            {
                context.Warnings.Add("FsklBone padding not empty");
            }
        }
    }
}
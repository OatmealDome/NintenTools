namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents a Level-of-Detail model of an FSHP subsection, describing the appearance of the polygon at different
    /// distances.
    /// </summary>
    public class FshpLodModel
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FshpLodModel"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FshpLodModel(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FshpLodModel"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of points in all visibility groups.
        /// </summary>
        public uint TotalPointCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FshpVisibilityGroup"/> instances, describing the start and length of
        /// parts to draw from the index buffer.
        /// </summary>
        public List<FshpVisibilityGroup> VisibilityGroups
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="FshpIndexBuffer"/> which references elements of the corresponding
        /// <see cref="FvtxVertexBuffer"/> to draw geometry with.
        /// </summary>
        public FshpIndexBuffer IndexBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of elements to skip in the parent's <see cref="FvtxVertexBuffer"/>.
        /// </summary>
        public uint SkipElements
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x00 != 4)
            {
                context.Warnings.Add("FshpLodModel.Unknown0x00 has unexpected value of " + Internal.Unknown0x00);
            }

            Internal.Unknown0x04 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x04 != 4)
            {
                context.Warnings.Add("FshpLodModel.Unknown0x04 has unexpected value of " + Internal.Unknown0x04);
            }

            TotalPointCount = context.Reader.ReadUInt32();
            Internal.VisibilityGroupCount = context.Reader.ReadUInt16();

            Internal.Unknown0x0E = context.Reader.ReadUInt16();
            if (Internal.Unknown0x0E != 0)
            {
                context.Warnings.Add("FshpLodModel.Unknown0x0E has unexpected value of " + Internal.Unknown0x0E);
            }

            Internal.VisibilityGroupOffset = context.Reader.ReadBfresOffset();
            Internal.IndexBufferOffset = context.Reader.ReadBfresOffset();
            SkipElements = context.Reader.ReadUInt32();

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadVisibilityGroup(context);
                LoadIndexBuffer(context);
            }
        }

        private void LoadVisibilityGroup(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.VisibilityGroupOffset.ToFile;
            VisibilityGroups = new List<FshpVisibilityGroup>(Internal.VisibilityGroupCount);
            for (int i = 0; i < Internal.VisibilityGroupCount; i++)
            {
                VisibilityGroups.Add(new FshpVisibilityGroup(context));
            }
        }

        private void LoadIndexBuffer(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.IndexBufferOffset.ToFile;
            IndexBuffer = new FshpIndexBuffer(context);
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FshpModel"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x00. The value always seems to be 4.
            /// </summary>
            public uint Unknown0x00
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0x04.
            /// </summary>
            public uint Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FshpVisibilityGroup"/> instances in the
            /// <see cref="VisibilityGroups"/> list.
            /// </summary>
            public ushort VisibilityGroupCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0x04.
            /// </summary>
            public ushort Unknown0x0E
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the offset to the first <see cref="FshpVisibilityGroup"/> instance.
            /// </summary>
            public BfresOffset VisibilityGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="FshpIndexBuffer"/> instance.
            /// </summary>
            public BfresOffset IndexBufferOffset
            {
                get;
                set;
            }
        }
    }
}
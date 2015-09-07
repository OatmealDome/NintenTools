namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents an index buffer of an <see cref="FshpLodModel"/> which references indices of the elements of the
    /// corresponding <see cref="FvtxVertexBuffer"/> to draw geometry with.
    /// </summary>
    public class FshpIndexBuffer
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FshpIndexBuffer"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FshpIndexBuffer(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FshpIndexBuffer"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the indices which reference elements from the <see cref="FvtxVertexBuffer"/> of the parent
        /// <see cref="FshpModel"/>.
        /// </summary>
        public ushort[] Data
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadUInt32();
            Internal.Size = context.Reader.ReadUInt32();
            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            Internal.Unknown0x0C = context.Reader.ReadUInt16();
            Internal.Unknown0x0E = context.Reader.ReadUInt16();
            Internal.Unknown0x10 = context.Reader.ReadUInt32();
            Internal.DataOffset = context.Reader.ReadBfresOffset();
            
            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadData(context);
            }
        }

        private void LoadData(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.DataOffset.ToFile;
            Data = context.Reader.ReadUInt16s((int)Internal.Size / 2);
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FshpIndexBuffer"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x00. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x00
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the size of the index buffer data in bytes.
            /// </summary>
            public uint Size
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0C. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x0C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0E. The value always seems to be 1.
            /// </summary>
            public ushort Unknown0x0E
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x10. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x10
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first index.
            /// </summary>
            public BfresOffset DataOffset
            {
                get;
                set;
            }
        }
    }
}
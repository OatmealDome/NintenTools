namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents an FVTX subsection in a FMDL section which describes the raw vertex data of a polygon.
    /// </summary>
    public class FvtxVertexBuffer
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FvtxVertexBuffer"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FvtxVertexBuffer(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FvtxVertexBuffer"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FvtxVertexData"/> instances which hold and describe the raw vertex data
        /// for one part of the polygon.
        /// </summary>
        public List<FvtxVertexData> Data
        {
            get;
            set;
        }
        
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FVTX")
            {
                throw new BfresException("FvtxVertexData identifier invalid");
            }

            Internal.AttributeCount = context.Reader.ReadByte();
            Internal.BufferCount = context.Reader.ReadByte();
            Internal.Index = context.Reader.ReadUInt16();
            Internal.NumberOfElements = context.Reader.ReadUInt32();

            Internal.Unknown0x0C = context.Reader.ReadUInt32();
            if (Internal.Unknown0x0C != 0x00000000 && Internal.Unknown0x0C != 0x01000000
                && Internal.Unknown0x0C != 0x02000000 && Internal.Unknown0x0C != 0x03000000
                && Internal.Unknown0x0C != 0x04000000)
            {
                context.Warnings.Add("FvtxVertexData.Unknown0x0C has unexpected value of " + Internal.Unknown0x0C);
            }

            Internal.AttributeArrayOffset = context.Reader.ReadBfresOffset();
            Internal.AttributeIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.BufferArrayOffset = context.Reader.ReadBfresOffset();

            if (context.Reader.ReadUInt32() != 0)
            {
                context.Warnings.Add("FvtxVertexData padding not empty");
            }

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadData(context);
                LoadAttributes(context);
            }
        }

        private void LoadData(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.BufferArrayOffset.ToFile;
            Data = new List<FvtxVertexData>(Internal.BufferCount);
            for (int i = 0; i < Internal.BufferCount; i++)
            {
                Data.Add(new FvtxVertexData(context));
            }
        }

        private void LoadAttributes(BfresLoaderContext context)
        {
            // Read in the attributes from the array and directly map them to the buffers.
            context.Reader.Position = Internal.AttributeArrayOffset.ToFile;
            
            for (int i = 0; i < Internal.AttributeCount; i++)
            {
                FvtxVertexAttribute attribute = new FvtxVertexAttribute(context);
                if (Data[attribute.Internal.BufferIndex].Attributes == null)
                {
                    Data[attribute.Internal.BufferIndex].Attributes = new List<FvtxVertexAttribute>();
                }
                Data[attribute.Internal.BufferIndex].Attributes.Add(attribute);
            }

            // Read in the index group.
            context.Reader.Position = Internal.AttributeIndexGroupOffset.ToFile;
            Internal.AttributeIndexGroup = new BfresIndexGroup(context);
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FvtxVertexBuffer"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> instances for the subsections.
            /// </summary>
            public byte AttributeCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FvtxVertexData"/> instances in the <see cref="Data"/> list.
            /// </summary>
            public byte BufferCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the buffer in the parents <see cref="FmdlSection"/> vertex data array list.
            /// </summary>
            public ushort Index
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of full vertices in each data buffer.
            /// </summary>
            public uint NumberOfElements
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0C. The value always seems to be 0x00000000, 0x01000000,
            /// 0x02000000, 0x03000000 or 0x04000000, and is normally 0x00000000.
            /// </summary>
            public uint Unknown0x0C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FvtxVertexAttribute"/>.
            /// </summary>
            public BfresOffset AttributeArrayOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing
            /// <see cref="FvtxVertexAttribute"/>.
            /// </summary>
            public BfresOffset AttributeIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing <see cref="FvtxVertexAttribute"/>.
            /// </summary>
            public BfresIndexGroup AttributeIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FvtxVertexData"/>.
            /// </summary>
            public BfresOffset BufferArrayOffset
            {
                get;
                set;
            }
        }
    }
}
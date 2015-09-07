namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using System.IO;
    using Vibeware.NintenTools.IO;
    using Vibeware.NintenTools.Maths;

    /// <summary>
    /// Represents the raw data array of a FVTX subsection, and describes the layout of each element in it.
    /// </summary>
    public class FvtxVertexData
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FvtxVertexData"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FvtxVertexData(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FvtxVertexData"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of one full element in the raw data array, in bytes.
        /// </summary>
        public uint Stride
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the raw data as a byte array.
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of attributes describing each part of an element in the raw data array.
        /// </summary>
        public List<FvtxVertexAttribute> Attributes
        {
            get;
            set;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets an array of <see cref="Vector3F"/> values representing the attributes data in each element.
        /// </summary>
        /// <param name="attribute">The <see cref="FvtxVertexAttribute"/>, which data is referenced.</param>
        /// <returns>An array containing the attributes data in each element.</returns>
        public Vector3F[] GetAttributeDataAsVector3F(FvtxVertexAttribute attribute)
        {
            List<Vector3F> typedData = new List<Vector3F>();
            
            using (BinaryDataReader reader = new BinaryDataReader(new MemoryStream(Data)))
            {
                reader.ByteOrder = ByteOrder.BigEndian;
                // Go through the array elements.
                for (int i = 0; i < Data.Length; i += (int)Stride)
                {
                    reader.Position = i + attribute.Offset;
                    switch (attribute.Format)
                    {
                        case FvtxVertexAttributeFormat.Three_32Bit_Float:
                            typedData.Add(reader.ReadVector3F());
                            break;
                        default:
                            throw new BfresException("Cannot retrieve attribute data as Vector3F: Unsupported format.");
                    }
                }
            }

            return typedData.ToArray();
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadUInt32();
            Internal.Size = context.Reader.ReadUInt32();
            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            Stride = context.Reader.ReadUInt16();
            Internal.Unknown0x0E = context.Reader.ReadUInt16();
            Internal.Unknown0x10 = context.Reader.ReadUInt32();
            Internal.Offset = context.Reader.ReadBfresOffset();

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadData(context);
            }
        }

        private void LoadData(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.Offset.ToFile;
            Data = context.Reader.ReadBytes((int)Internal.Size);
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FvtxVertexData"/> instance.
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
            /// Gets or sets the size of the buffer data array in bytes.
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
            /// Gets or sets the offset to the first byte of the vertex buffer data.
            /// </summary>
            public BfresOffset Offset
            {
                get;
                set;
            }
        }
    }
}
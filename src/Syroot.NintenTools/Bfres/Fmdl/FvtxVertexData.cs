namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Syroot.IO;
    using Syroot.NintenTools.IO;
    using Syroot.NintenTools.Maths;

    /// <summary>
    /// Represents the raw data array of a FVTX subsection, and describes the layout of each element in it.
    /// </summary>
    /// <remarks>This is named "Buffer" in the Mario Kart 8 wiki, but since it mostly describes raw data compared to the
    /// </remarks>
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
        /// Gets an array of <see cref="Vector2F"/> values representing the attributes data in each element.
        /// </summary>
        /// <param name="attribute">The <see cref="FvtxVertexAttribute"/>, which data is referenced.</param>
        /// <returns>An array containing the attributes data in each element.</returns>
        public Vector2F[] GetAttributeDataAsVector2F(FvtxVertexAttribute attribute)
        {
            List<Vector2F> typedData = new List<Vector2F>();

            using (BinaryDataReader reader = new BinaryDataReader(new MemoryStream(Data)))
            {
                reader.ByteOrder = ByteOrder.BigEndian;
                // Go through the array elements.
                for (int i = 0; i < Data.Length; i += (int)Stride)
                {
                    reader.Position = i + attribute.Offset;
                    switch (attribute.Format)
                    {
                        case FvtxVertexAttributeFormat.Two_32Bit_Float:
                            typedData.Add(reader.ReadVector2F());
                            break;
                        case FvtxVertexAttributeFormat.Two_16Bit_Normalized:
                            ushort x = reader.ReadUInt16();
                            ushort y = reader.ReadUInt16();
                            typedData.Add(new Vector2F(x / 65535f, y / 65535f));
                            break;
                        default:
                            throw new BfresException("Cannot retrieve attribute data as Vector2F: Mismatching format.");
                    }
                }
            }

            return typedData.ToArray();
        }

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
                        case FvtxVertexAttributeFormat.Three_10Bit_Signed:
                            // Actually 32 bits, first 2 bits always 0b01. Packed like 01XXXXXXXXXXYYYYYYYYYYZZZZZZZZZZ.
                            uint packedBytes = reader.ReadUInt32();
                            short x = (short)((packedBytes >> 20) & 0x3FF);
                            short y = (short)((packedBytes >> 10) & 0x3FF);
                            short z = (short)(packedBytes & 0x3FF);
                            // Divide by 511 to get the float value.
                            typedData.Add(new Vector3F(x / 511f, y / 511f, z / 511f));
                            break;
                        default:
                            throw new BfresException("Cannot retrieve attribute data as Vector3F: Mismatching format.");
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
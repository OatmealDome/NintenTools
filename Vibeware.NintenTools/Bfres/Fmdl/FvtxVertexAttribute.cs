namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents the layout of one part of an element in the raw data array of a <see cref="FvtxVertData"/> instance.
    /// </summary>
    [DebuggerDisplay("Vertex Attribute {Name} {Format}")]
    public class FvtxVertexAttribute
    {
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FvtxVertexAttribute"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the index of the <see cref="FvtxVertexData"/> this attribute belongs to.
            /// </summary>
            public byte BufferIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="Name"/>.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }
        }

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FvtxVertexAttribute"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FvtxVertexAttribute(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FvtxVertexAttribute"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the element, describing the meaning of the value.
        /// </summary>
        /// <remarks>
        /// The following naming convention seems to be used for attributes:
        /// _p0  position0     Position
        /// _n0  normal0       Normal for lighting
        /// _t0  tangent0      Tangent for advanced lighting
        /// _b0  binormal0     Binormal for advanced lighting
        /// _w0  blendweight0  Unknown
        /// _i0  blendindex0   Unknown
        /// _u0  uv0           Texture coordinates, layer 0
        /// _u1  uv1           Texture coordinates, layer 1
        /// _u2  uv2           Texture coordinates, layer 2
        /// _u3  uv3           Texture coordinates, layer 3
        /// _c0  color0        Color for shadow mapping
        /// _c1  color1        Color for shadow mapping
        /// </remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset into the element where this part starts, from the elements start, in bytes.
        /// </summary>
        public uint Offset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the layout of the raw data which represents this attributes value.
        /// </summary>
        public FvtxVertexAttributeFormat Format
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

            uint indexAndOffset = context.Reader.ReadUInt32();
            Internal.BufferIndex = (byte)(indexAndOffset & 0xFF000000);
            Offset = indexAndOffset & 0x00FFFFFF;

            Format = (FvtxVertexAttributeFormat)context.Reader.ReadUInt32();
        }
    }

    /// <summary>
    /// Represents the known layouts of a part in a vertex buffer element.
    /// </summary>
    public enum FvtxVertexAttributeFormat : uint
    {
        /// <summary>
        /// 2 8-bit values, representing numbers between 0 and 1.
        /// </summary>
        Two_8Bit_Normalized = 0x00000004,

        /// <summary>
        /// 2 16-bit values, representing numbers between 0 and 1.
        /// </summary>
        Two_16Bit_Normalized = 0x00000007,

        /// <summary>
        /// 4 signed 8-bit values.
        /// </summary>
        Four_8Bit_Signed = 0x0000020A,

        /// <summary>
        /// 3 signed 10-bit values Divide by 511 to get decimal value. Top 2 bits are always 0x0B01.
        /// </summary>
        Three_10Bit_Signed = 0x0000020B,

        /// <summary>
        /// 2 32-bit floating point values.
        /// </summary>
        Two_32Bit_Float = 0x0000080D,

        /// <summary>
        /// 4 16-bit half precision floating point values.
        /// </summary>
        Four_16Bit_Float = 0x0000080F,

        /// <summary>
        /// 3 32-bit floating point values.
        /// </summary>
        Three_32Bit_Float = 0x00000811
    }
}
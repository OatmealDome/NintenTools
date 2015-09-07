namespace Vibeware.NintenTools.Bfres.Ftex
{
    using Vibeware.NintenTools.Gx2;

    /// <summary>
    /// Represents an FTEX section in a BFRES file which describes texture data.
    /// </summary>
    public class FtexSection
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FtexSection"/> for the given <see cref="BfresFile"/>.
        /// The stream of the file has to be positioned at the beginning of the data.
        /// </summary>
        /// <param name="bfresFile">The BFRES file providing the context.</param>
        internal FtexSection(BfresFile bfresFile)
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Get or sets the shape of the texture.
        /// </summary>
        public SurfaceDim SurfaceDim
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the texture.
        /// </summary>
        public uint Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the texture.
        /// </summary>
        public uint Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bit depth of the texture.
        /// </summary>
        public uint Depth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of mipmaps in the <see cref="MipmapData"/>.
        /// </summary>
        public uint MipmapCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format in which the <see cref="Data"/> is stored.
        /// </summary>
        public SurfaceFormat Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of samples for the surface.
        /// </summary>
        public AntiAliasMode AntiAliasMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets how the surface may be used.
        /// </summary>
        public SurfaceUse Usage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown variable at offset 0x28.
        /// </summary>
        public uint Unknown0x28
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mipmap size.
        /// </summary>
        public uint MipmapSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown variable at offset 0x30.
        /// </summary>
        public uint Unknown0x30
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired tiling mode for the surface.
        /// </summary>
        public TileMode TileMode
        {
            get;
            set;
        }
        
        public uint Swizzle
        {
            get;
            set;
        }

        public uint Alignment
        {
            get;
            set;
        }

        public uint Pitch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown byte array at offset 0x44 of the length 0x6C.
        /// </summary>
        public byte[] Unknown0x44
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the texture data.
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mipmap data.
        /// </summary>
        public byte[] MipmapData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown variable at offset 0xB8.
        /// </summary>
        public uint Unknown0xB8
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown variable at offset 0xBC.
        /// </summary>
        public uint Unknown0xBC
        {
            get;
            set;
        }
    }
}
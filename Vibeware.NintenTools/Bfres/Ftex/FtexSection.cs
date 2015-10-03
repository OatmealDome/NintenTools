namespace Vibeware.NintenTools.Bfres.Ftex
{
    using Vibeware.NintenTools.Gx2;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents an FTEX section in a BFRES file which describes texture data.
    /// </summary>
    public class FtexSection
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FtexSection"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FtexSection(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FtexSection"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

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

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FTEX")
            {
                throw new BfresException("FtexSection identifier invalid");
            }

            // Read section properties.
            SurfaceDim = (SurfaceDim)context.Reader.ReadInt32();
            Width = context.Reader.ReadUInt32();
            Height = context.Reader.ReadUInt32();
            Depth = context.Reader.ReadUInt32();

            MipmapCount = context.Reader.ReadUInt32();
            Format = (SurfaceFormat)context.Reader.ReadInt32();
            AntiAliasMode = (AntiAliasMode)context.Reader.ReadInt32();
            Usage = (SurfaceUse)context.Reader.ReadInt32();

            Internal.DataLength = context.Reader.ReadUInt32();
            Internal.Unknown0x28 = context.Reader.ReadUInt32();
            Internal.MipmapSize = context.Reader.ReadUInt32();
            Internal.Unknown0x30 = context.Reader.ReadUInt32();

            TileMode = (TileMode)context.Reader.ReadUInt32();
            Swizzle = context.Reader.ReadUInt32();
            Alignment = context.Reader.ReadUInt32();
            Pitch = context.Reader.ReadUInt32();

            Internal.Unknown0x44 = context.Reader.ReadBytes(0x6C);
            Internal.DataOffset = context.Reader.ReadBfresOffset();
            Internal.MipmapOffset = context.Reader.ReadBfresOffset();

            Internal.Unknown0xB8 = context.Reader.ReadUInt32();
            Internal.Unknown0xBC = context.Reader.ReadUInt32();

            LoadData(context);
            LoadMipmapData(context);
        }

        private void LoadData(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.DataOffset.ToFile;
            Data = context.Reader.ReadBytes((int)Internal.DataLength);
        }

        private void LoadMipmapData(BfresLoaderContext context)
        {
            if (!Internal.MipmapOffset.IsEmpty)
            {
                context.Reader.Position = Internal.MipmapOffset.ToFile;
                MipmapData = context.Reader.ReadBytes((int)Internal.MipmapSize);
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FtexSection"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------
            
            /// <summary>
            /// Gets or sets the length of the texture data in bytes.
            /// </summary>
            public uint DataLength
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
            /// Gets or sets the length of the mipmap texture data in bytes.
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
            /// Gets or sets an unknown byte array at offset 0x44 of the length 0x6C.
            /// </summary>
            public byte[] Unknown0x44
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the texture data relative to the position of this field.
            /// </summary>
            public BfresOffset DataOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to mipmap texture data relative to the position of this field. It is 0 if mipmap
            /// data is not present.
            /// </summary>
            public BfresOffset MipmapOffset
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
}
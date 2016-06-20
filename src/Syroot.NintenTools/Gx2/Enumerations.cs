namespace Syroot.NintenTools.Gx2
{
    // This file contains definitions originating from gx2Enum.h, converted into C#.

    /// <summary>
    /// Indicates desired texture, color-buffer, depth-buffer, or scan-buffer format.
    ///
    /// The following letters indicate the possible uses:
    /// T=texture, C=color-buffer, D=depth/stencil-buffer, S=scan-buffer.
    ///
    /// There are some formats with the same enum value, but different use labels. These are provided as a convenience
    /// to explain the type information for each use.
    ///
    /// Type conversion options:
    /// - UNorm (0): surface unsigned integer is converted to/from [0.0, 1.0] in shader
    /// - UInt  (1): surface unsigned integer is copied to/from shader as unsigned int
    /// - SNorm (2): surface signed integer is converted to/from [-1.0, 1.0] in shader
    /// - SInt  (3): surface signed integer is copied to/from shader as signed int
    /// - SRGB  (4): SRGB degamma performed on surface read, then treated as UNORM;
    ///              SRGB gamma is performed on surface write
    /// - Float (8): surface float is copied to/from shader as float
    ///
    /// Note: As textures, all UInt/SInt formats may be point-sampled only!
    ///
    /// The numbers in the names indicate the number of bits per channel, as well as how many channels are present. An
    /// "X" in front of a number indicates padding bits that are present, but do not map to any channel.
    ///
    /// Texture color channel mappings:
    /// - 1-channel formats map to R [GBA are undefined]
    /// - 2-channel formats map to RG [BA are undefined]
    /// - 3-channel formats map to RGB [A is undefined]
    /// - 4-channel formats map to RGBA
    ///
    /// Channel mapping can be changed using the GX2InitTextureCompSel API. We advise you avoid referring to channels
    /// that don't exist in the format. You should use the component select to choose constant values in those cases.
    /// The default component selectors in GX2InitTexture map:
    /// - 1-channel formats to R001
    /// - 2-channel formats to RG01
    /// - 3-channel formats to RGB1
    /// - 4-channel formats to RGBA
    ///
    /// To understand exact component bit placement, you must first understand the basic machine unit that the
    /// components are packed into. If each component fits into a single unit, then the order is simply R,G,B,A. If
    /// multiple components are packed into a single unit, then the components are packed in order starting from the LSB
    /// end. In all cases, multi-byte machine units are then written out in little-endian format.
    ///
    /// Note 1: It is not presently possible to switch between depth and color buffer uses for the same surface. This
    /// requires a retiling, since the tile formats are different and incompatible. The texture unit can read
    /// depth-tiled buffers (except for D24_S8 format). The D24_S8 format requires tile-conversion before it can be read
    /// by the texture unit. Note that the two components have different number formats, and only the depth part can be
    /// sampled with any filter more complex than point-sampling. It is needed to use T_R24_UNorm_X8 for reading depth
    /// buffer as texture and T_X24_G8_UInt for reading stencil buffer as texture. See
    /// <see cref="ConvertDepthBufferToTextureSurface()"/> for more information.
    ///
    /// Note 2: Similar to depth format D_D24_S8_UNorm and texture formats T_R24_UNorm_X8 and T_X24_G8_UInt, format
    /// D_D32_Float_S8_UInt_X24 is a depth/stencil buffer format while T_R32_Float_X8_X24 and T_X32_G8_UInt_X24 are
    /// texture formats used to read the depth and stencil data, respectively. See
    /// <see cref="ConvertDepthBufferToTextureSurface()"/> for more information.
    ///
    /// Note 3: The NV12 format is a special case for video. It actually consists of two surfaces (an 8-bit surface
    /// &amp; a 1/4-size 16-bit surface). It is only usable in certain situations.
    ///
    /// Final note: there may be additional restrictions not yet specified.
    /// </summary>
    /// <remarks>Native type: GX2SurfaceFormat</remarks>
    public enum SurfaceFormat : int
    {
        Invalid = 0x00000000,
        TC_R8_UNorm = 0x00000001,
        TC_R8_UInt = 0x00000101,
        TC_R8_SNorm = 0x00000201,
        TC_R8_SInt = 0x00000301,
        T_R4_G4_UNorm = 0x00000002,
        TCD_R16_UNorm = 0x00000005,
        TC_R16_UInt = 0x00000105,
        TC_R16_SNorm = 0x00000205,
        TC_R16_SInt = 0x00000305,
        TC_R16_Float = 0x00000806,
        TC_R8_G8_UNorm = 0x00000007,
        TC_R8_G8_UInt = 0x00000107,
        TC_R8_G8_SNorm = 0x00000207,
        TC_R8_G8_SInt = 0x00000307,
        TCS_R5_G6_B5_UNorm = 0x00000008,
        TC_R5_G5_B5_A1_UNorm = 0x0000000A,
        TC_R4_G4_B4_A4_UNorm = 0x0000000B,
        TC_A1_B5_G5_R5_UNorm = 0x0000000C,
        TC_R32_UInt = 0x0000010D,
        TC_R32_SInt = 0x0000030D,
        TCD_R32_Float = 0x0000080E,
        TC_R16_G16_UNorm = 0x0000000F,
        TC_R16_G16_UInt = 0x0000010F,
        TC_R16_G16_SNorm = 0x0000020F,
        TC_R16_G16_SInt = 0x0000030F,
        TC_R16_G16_Float = 0x00000810,
        D_D24_S8_UNorm = 0x00000011,
        T_R24_UNorm_X8 = 0x00000011,
        T_X24_G8_UInt = 0x00000111,
        D_D24_S8_Float = 0x00000811,
        TC_R11_G11_B10_Float = 0x00000816,
        TCS_R10_G10_B10_A2_UNorm = 0x00000019,
        TC_R10_G10_B10_A2_UInt = 0x00000119,
        TC_R10_G10_B10_A2_SNorm = 0x00000219,                           
        TC_R10_G10_B10_A2_SInt = 0x00000319,
        TCS_R8_G8_B8_A8_UNorm = 0x0000001A,
        TC_R8_G8_B8_A8_UInt = 0x0000011A,
        TC_R8_G8_B8_A8_SNorm = 0x0000021A,
        TC_R8_G8_B8_A8_SInt = 0x0000031A,
        TCS_R8_G8_B8_A8_SRGB = 0x0000041A,
        TCS_A2_B10_G10_R10_UNorm = 0x0000001B,
        TC_A2_B10_G10_R10_UInt = 0x0000011B,
        D_D32_Float_S8_UInt_X24 = 0x0000081C,
        T_R32_Float_X8_X24 = 0x0000081C,
        T_X32_G8_UInt_X24 = 0x0000011C,
        TC_R32_G32_UInt = 0x0000011D,
        TC_R32_G32_SInt = 0x0000031D,
        TC_R32_G32_Float = 0x0000081E,
        TC_R16_G16_B16_A16_UNorm = 0x0000001F,
        TC_R16_G16_B16_A16_UInt = 0x0000011F,
        TC_R16_G16_B16_A16_SNorm = 0x0000021F,
        TC_R16_G16_B16_A16_SInt = 0x0000031F,
        TC_R16_G16_B16_A16_Float = 0x00000820,
        TC_R32_G32_B32_A32_UInt = 0x00000122,
        TC_R32_G32_B32_A32_SInt = 0x00000322,
        TC_R32_G32_B32_A32_Float = 0x00000823,
        T_BC1_UNorm = 0x00000031,
        T_BC1_SRGB = 0x00000431,
        T_BC2_UNorm = 0x00000032,
        T_BC2_SRGB = 0x00000432,
        T_BC3_UNorm = 0x00000033,
        T_BC3_SRGB = 0x00000433,
        T_BC4_UNorm = 0x00000034,
        T_BC4_SNorm = 0x00000234,
        T_BC5_UNorm = 0x00000035,
        T_BC5_SNorm = 0x00000235,
        T_NV12_UNorm = 0x00000081
    }

    /// <summary>
    /// Indicates the desired tiling mode for a surface. You should use only <see cref="Default"/> in most cases. Don't
    /// use other modes unless you know what you're doing!
    /// </summary>
    /// <remarks>Native type: GX2TileMode</remarks>
    public enum TileMode : int
    {
        /// <summary>
        /// Driver will choose best mode.
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// Typically not supported by HW.
        /// </summary>
        LinearSpecial = 0x00000010,

        /// <summary>
        /// Supported by HW, but not fast.
        /// </summary>
        LinearAligned = 0x00000001,
        OneDTiledThin1 = 0x00000002,
        OneDTiledThick = 0x00000003,

        /// <summary>
        /// (A typical default, but not always.)
        /// </summary>
        TwoDTiledThin1 = 0x00000004,
        TwoDTiledThin2 = 0x00000005,
        TwoDTiledThin4 = 0x00000006,
        TwoDTiledThick = 0x00000007,
        TwoBTiledThin1 = 0x00000008,
        TwoBTiledThin2 = 0x00000009,
        TwoBTiledThin4 = 0x0000000A,
        TwoBTiledThick = 0x0000000B,
        ThreeDTiledThin1 = 0x0000000C,
        ThreeDTiledThick = 0x0000000D,
        ThreeBTiledThin1 = 0x0000000E,
        ThreeBTiledThick = 0x0000000F,
    }

    /// <summary>
    /// Indicates how a given surface may be used.
    /// A &quot;final&quot; TV render target is one that will be copied to a TV scan buffer. It needs to be designed to
    /// handle certain display corner cases. (When a HD surface must be scaled down to display in NTSC/PAL.)
    /// </summary>
    /// <remarks>Native type: GX2SurfaceUse</remarks>
    public enum SurfaceUse : int
    {
        Texture = 1 << 0,
        ColorBuffer = 1 << 1,
        DepthBuffer = 1 << 2,

        /// <summary>
        /// Internal use only.
        /// </summary>
        ScanBuffer = 1 << 4,

        /// <summary>
        /// Modifier, designates a final TV render target.
        /// </summary>
        Ftv = 1 << 31,

        ColorBufferTexture = ColorBuffer | Texture,
        DepthBufferTexture = DepthBuffer | Texture,

        ColorBufferFtv = ColorBuffer | Ftv,
        ColorBufferTextureFtv = ColorBufferTexture | Ftv
    }

    /// <summary>
    /// Indicates the &quot;shape&quot; of a given surface or texture.
    /// </summary>
    /// <remarks>Native type: GX2SurfaceDim</remarks>
    public enum SurfaceDim : int
    {
        OneDimensional = 0,
        TwoDimensional = 1,
        ThreeDimensional = 2,
        Cube = 3,
        OneDimensionalArray = 4,
        TwoDimensionalArray = 5,
        TwoDimensionalMsaa = 6,
        TwoDimensionalMsaaArray = 7
    }

    /// <summary>
    /// Indicates the AA mode (number of samples) for a surface.
    /// </summary>
    /// <remarks>Native type: GX2AAMode</remarks>
    public enum AntiAliasMode : int
    {
        OneSample = 0,
        TwoSamples = 1,
        FourSamples = 2,
        EightSamples = 3
    }
}

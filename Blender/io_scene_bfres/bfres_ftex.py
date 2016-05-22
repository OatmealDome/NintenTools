import enum
from .bfres_common import BfresOffset, BfresNameOffset, IndexGroup

class FtexSection:
    class Header:
        class SurfaceDim(enum.IntEnum):
            # GX2SurfaceDim
            OneDimensional          = 0
            TwoDimensional          = 1
            ThreeDimensional        = 2
            Cube                    = 3
            OneDimensionalArray     = 4
            TwoDimensionalArray     = 5
            TwoDimensionalMsaa      = 6
            TwoDimensionalMsaaArray = 7

        class SurfaceFormat(enum.IntEnum):
            # GX2SurfaceFormat
            Invalid                  = 0x00000000
            TC_R8_UNorm              = 0x00000001
            TC_R8_UInt               = 0x00000101
            TC_R8_SNorm              = 0x00000201
            TC_R8_SInt               = 0x00000301
            T_R4_G4_UNorm            = 0x00000002
            TCD_R16_UNorm            = 0x00000005
            TC_R16_UInt              = 0x00000105
            TC_R16_SNorm             = 0x00000205
            TC_R16_SInt              = 0x00000305
            TC_R16_Float             = 0x00000806
            TC_R8_G8_UNorm           = 0x00000007
            TC_R8_G8_UInt            = 0x00000107
            TC_R8_G8_SNorm           = 0x00000207
            TC_R8_G8_SInt            = 0x00000307
            TCS_R5_G6_B5_UNorm       = 0x00000008
            TC_R5_G5_B5_A1_UNorm     = 0x0000000A
            TC_R4_G4_B4_A4_UNorm     = 0x0000000B
            TC_A1_B5_G5_R5_UNorm     = 0x0000000C
            TC_R32_UInt              = 0x0000010D
            TC_R32_SInt              = 0x0000030D
            TCD_R32_Float            = 0x0000080E
            TC_R16_G16_UNorm         = 0x0000000F
            TC_R16_G16_UInt          = 0x0000010F
            TC_R16_G16_SNorm         = 0x0000020F
            TC_R16_G16_SInt          = 0x0000030F
            TC_R16_G16_Float         = 0x00000810
            D_D24_S8_UNorm           = 0x00000011
            T_R24_UNorm_X8           = 0x00000011
            T_X24_G8_UInt            = 0x00000111
            D_D24_S8_Float           = 0x00000811
            TC_R11_G11_B10_Float     = 0x00000816
            TCS_R10_G10_B10_A2_UNorm = 0x00000019
            TC_R10_G10_B10_A2_UInt   = 0x00000119
            TC_R10_G10_B10_A2_SNorm  = 0x00000219
            TC_R10_G10_B10_A2_SInt   = 0x00000319
            TCS_R8_G8_B8_A8_UNorm    = 0x0000001A
            TC_R8_G8_B8_A8_UInt      = 0x0000011A
            TC_R8_G8_B8_A8_SNorm     = 0x0000021A
            TC_R8_G8_B8_A8_SInt      = 0x0000031A
            TCS_R8_G8_B8_A8_SRGB     = 0x0000041A
            TCS_A2_B10_G10_R10_UNorm = 0x0000001B
            TC_A2_B10_G10_R10_UInt   = 0x0000011B
            D_D32_Float_S8_UInt_X24  = 0x0000081C
            T_R32_Float_X8_X24       = 0x0000081C
            T_X32_G8_UInt_X24        = 0x0000011C
            TC_R32_G32_UInt          = 0x0000011D
            TC_R32_G32_SInt          = 0x0000031D
            TC_R32_G32_Float         = 0x0000081E
            TC_R16_G16_B16_A16_UNorm = 0x0000001F
            TC_R16_G16_B16_A16_UInt  = 0x0000011F
            TC_R16_G16_B16_A16_SNorm = 0x0000021F
            TC_R16_G16_B16_A16_SInt  = 0x0000031F
            TC_R16_G16_B16_A16_Float = 0x00000820
            TC_R32_G32_B32_A32_UInt  = 0x00000122
            TC_R32_G32_B32_A32_SInt  = 0x00000322
            TC_R32_G32_B32_A32_Float = 0x00000823
            T_BC1_UNorm              = 0x00000031
            T_BC1_SRGB               = 0x00000431
            T_BC2_UNorm              = 0x00000032
            T_BC2_SRGB               = 0x00000432
            T_BC3_UNorm              = 0x00000033
            T_BC3_SRGB               = 0x00000433
            T_BC4_UNorm              = 0x00000034
            T_BC4_SNorm              = 0x00000234
            T_BC5_UNorm              = 0x00000035
            T_BC5_SNorm              = 0x00000235
            T_NV12_UNorm             = 0x00000081

        class AntiAliasMode(enum.IntEnum):
            # GX2AAMode
            OneSample    = 0
            TwoSamples   = 1
            FourSamples  = 2
            EightSamples = 3

        class SurfaceUse(enum.IntEnum):
            # GX2SurfaceUse
            Texture               = 1 << 0
            ColorBuffer           = 1 << 1
            DepthBuffer           = 1 << 2
            ScanBuffer            = 1 << 4
            Ftv                   = 1 << 31
            ColorBufferTexture    = ColorBuffer | Texture
            DepthBufferTexture    = DepthBuffer | Texture
            ColorBufferFtv        = ColorBuffer | Ftv
            ColorBufferTextureFtv = ColorBufferTexture | Ftv

        class TileMode(enum.IntEnum):
            # GX2TileMode
            Default          = 0x00000000
            LinearSpecial    = 0x00000010
            LinearAligned    = 0x00000001
            OneDTiledThin1   = 0x00000002
            OneDTiledThick   = 0x00000003
            TwoDTiledThin1   = 0x00000004
            TwoDTiledThin2   = 0x00000005
            TwoDTiledThin4   = 0x00000006
            TwoDTiledThick   = 0x00000007
            TwoBTiledThin1   = 0x00000008
            TwoBTiledThin2   = 0x00000009
            TwoBTiledThin4   = 0x0000000A
            TwoBTiledThick   = 0x0000000B
            ThreeDTiledThin1 = 0x0000000C
            ThreeDTiledThick = 0x0000000D
            ThreeBTiledThin1 = 0x0000000E
            ThreeBTiledThick = 0x0000000F

        def __init__(self, reader):
            if reader.read_raw_string(4) != "FTEX":
                raise AssertionError("Invalid FTEX section header.")
            self.dim = reader.read_uint32() # self.SurfaceDim
            self.width = reader.read_uint32()
            self.height = reader.read_uint32()
            self.depth = reader.read_uint32()
            self.mipmap_count = reader.read_uint32()
            self.format = reader.read_uint32() # self.SurfaceFormat
            self.anti_alias_mode = reader.read_uint32() # self.AntiAliasMode
            self.usage = reader.read_uint32() # self.SurfaceUse
            self.data_size = reader.read_uint32() # in bytes
            self.unknown0x28 = reader.read_uint32()
            self.mipmap_data_size = reader.read_uint32() # in bytes
            self.unknown0x30 = reader.read_uint32()
            self.tile_mode = reader.read_uint32() # self.TileMode
            self.swizzle = reader.read_uint32()
            self.alignment = reader.read_uint32()
            self.pitch = reader.read_uint32()
            self.unknown0x6c = reader.read_uint32s(27)
            self.data_offset = BfresOffset(reader)
            self.mipmap_data_offset = BfresOffset(reader)
            self.unknown0xb8 = reader.read_uint32() # Seem to change per file.
            self.unknown0xbc = reader.read_uint32()

    def __init__(self, reader):
        self.header = self.Header(reader)
        # Load the raw data.
        reader.seek(self.header.data_offset.to_file)
        self.data = reader.read_bytes(self.header.data_size)
        # Load the raw mipmap data.
        reader.seek(self.header.mipmap_data_offset.to_file)
        self.mipmap_data = reader.read_bytes(self.header.mipmap_data_size)

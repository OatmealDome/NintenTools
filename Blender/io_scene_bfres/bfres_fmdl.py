import enum
from .bfres_common import BfresOffset, BfresNameOffset, IndexGroup
from .binary_io import BinaryReader

class FmdlSection:
    class Header:
        def __init__(self, reader):
            if reader.read_raw_string(4) != "FMDL":
                raise AssertionError("Invalid FMDL section header.")
            self.file_name_offset = BfresNameOffset(reader)
            self.data_offset = BfresOffset(reader) # Wiki says end of string table, but that's where FMDL data begins.
            self.fskl_offset = BfresOffset(reader)
            self.fvtx_array_offset = BfresOffset(reader)
            self.fshp_index_group_offset = BfresOffset(reader)
            self.fmat_index_group_offset = BfresOffset(reader)
            self.param_index_group_offset = BfresOffset(reader)
            self.fvtx_count = reader.read_uint16()
            self.fshp_count = reader.read_uint16()
            self.fmat_count = reader.read_uint16()
            self.param_count = reader.read_uint16()
            self.unknown0x28 = reader.read_uint32() # Maybe an unused face count.

    def __init__(self, reader):
        self.header = self.Header(reader)
        # Read the FSKL subsection.
        reader.seek(self.header.fskl_offset.to_file)
        self.fskl_subsection = FsklSubsection(reader)
        # Read the FVTX subsections.
        reader.seek(self.header.fvtx_array_offset.to_file)
        self.fvtx_subsections = []
        for i in range(0, self.header.fvtx_count):
            self.fvtx_subsections.append(FvtxSubsection(reader))

class FsklSubsection:
    class Header:
        def __init__(self, reader):
            if reader.read_raw_string(4) != "FSKL":
                raise AssertionError("Invalid FSKL subsection header.")
            self.unknown0x04 = reader.read_uint16()
            self.unknown0x06 = reader.read_uint16() # 0x1100, 0x1200
            self.bone_count = reader.read_uint16()
            self.inv_count = reader.read_uint16() # Count of elements in inverse index and matrix arrays.
            self.extra_index_count = reader.read_uint16() # Additional elements in inverse index array.
            self.unknown0x0e = reader.read_uint16()
            self.bone_index_group_array_offset = BfresOffset(reader)
            self.bone_array_offset = BfresOffset(reader)
            self.inv_index_array_offset = BfresOffset(reader)
            self.inv_matrix_array_offset = BfresOffset(reader)

    class Bone:
        CHILD_BONE_COUNT = 4 # Wiki says parent bones, but where does that make sense to have multiple parents?

        def __init__(self, reader):
            self.name_offset = BfresNameOffset(reader)
            self.index = reader.read_uint16()
            self.child_indices = []
            for i in range(0, self.CHILD_BONE_COUNT):
                self.child_indices.append(reader.read_uint16()) # 0xFFFF for no child.
            self.unknown0x0e = reader.read_uint16()
            self.flags = reader.read_uint16() # Unknown purpose.
            self.unknown0x12 = reader.read_uint16() # 0x1001
            self.scale = reader.read_vector3f()
            self.rotation = reader.read_quaternion()
            self.translation = reader.read_vector3f()
            self.padding = reader.read_uint32() # 0x00000000.

    def __init__(self, reader):
        self.header = self.Header(reader)
        # Read the bone index group.
        reader.seek(self.header.bone_index_group_array_offset.to_file)
        self.bone_index_group = IndexGroup(reader, lambda r: self.Bone(r))
        # Read inverse indices and matrices.
        reader.seek(self.header.inv_index_array_offset.to_file)
        self.inv_indices = []
        for i in range(0, self.header.inv_count + self.header.extra_index_count):
            self.inv_indices.append(reader.read_uint16())
        reader.seek(self.header.inv_matrix_array_offset.to_file)
        self.inv_matrices = []
        for i in range(0, self.header.inv_count):
            self.inv_matrices.append(reader.read_matrix4x3())

class FvtxSubsection:
    class Header:
        def __init__(self, reader):
            if reader.read_raw_string(4) != "FVTX":
                raise AssertionError("Invalid FVTX subsection header.")
            self.attribute_count = reader.read_byte()
            self.buffer_count = reader.read_byte()
            self.index = reader.read_uint16() # The index in the FMDL FVTX array.
            self.vertex_count = reader.read_uint32()
            self.unknown0x0c = reader.read_uint32() # 0x00000000 (normally), 0x04000000
            self.attribute_array_offset = BfresOffset(reader)
            self.attribute_index_group_offset = BfresOffset(reader)
            self.buffer_array_offset = BfresOffset(reader)
            self.padding = reader.read_uint32() # 0x00000000

    class Attribute:
        class Format(enum.IntEnum):
            Two8BitNormalized  = 0x00000004
            Two16BitNormalized = 0x00000007
            Four8BitSigned     = 0x0000020a
            Three10BitSigned   = 0x0000020b
            Two32BitFloat      = 0x0000080d
            Four16BitFloat     = 0x0000080f
            Three32BitFloat    = 0x00000811

        def __init__(self, reader):
            self.name_offset = BfresOffset(reader)
            index_and_offset = reader.read_uint32() # XXYYYYYY, where X is the buffer index and Y the offset.
            self.buffer_index = (index_and_offset & 0xFF000000) >> 24
            self.element_offset = index_and_offset & 0x00FFFFFF # Offset in each element.
            self.format = reader.read_uint32()

    class Buffer:
        def __init__(self, reader):
            self.unknown0x00 = reader.read_uint32() # 0x00000000
            self.size_in_bytes = reader.read_uint32()
            self.unknown0x08 = reader.read_uint32() # 0x00000000
            self.stride = reader.read_uint16() # Size of each element in the buffer
            self.unknown0x0e = reader.read_uint16() # 0x0001
            self.unknown0x10 = reader.read_uint32() # 0x00000000
            self.data_offset = BfresOffset(reader)
            # Read in the raw data.
            current_pos = reader.tell()
            reader.seek(self.data_offset.to_file)
            self.data = reader.read_bytes(self.size_in_bytes)
            reader.seek(current_pos)

    def __init__(self, reader):
        self.header = self.Header(reader)
        # Read the attribute index group.
        current_pos = reader.tell()
        reader.seek(self.header.attribute_index_group_offset.to_file)
        self.attribute_index_group = IndexGroup(reader, lambda r: self.Attribute(r))
        # Read the buffer array.
        reader.seek(self.header.buffer_array_offset.to_file)
        self.buffers = []
        for i in range(0, self.header.buffer_count):
            self.buffers.append(self.Buffer(reader))
        # Seek back as FVTX headers are read sequentially.
        reader.seek(current_pos)
import enum
import mathutils
from .binary_io import BinaryReader
from .bfres_common import BfresOffset, BfresNameOffset, IndexGroup
from .bfres_fmdl import FmdlSection

'''
Hierarchically visualized, the layout of a BFRES file is as follows:
- BFRES
  - 12 Index Groups
    - Index Group 0
      - FMDL[] (mostly only 1 of these)
        - FSKL
        - FVTX[]
        - ...
    - Index Group 1
      - FTEX[]
    - ...
  - String Table

However, this is just a silly simplification. The BFRES file is by far not as sequential as expectable from the layout
given above. Actually, the headers of the specific sections just point around in the file (relative to themselves, not
the file), and strings are globally collected in a file-wide string table. Data is found in an order as follows:
- BFRES Header
- Index Groups
- Headers of the sections referenced by the Index Groups
- String Table
- Data referenced by the section headers

Index Groups are technically binary trees allowing a quick named lookup of elements in a corresponding array. Combined
with the array, they are easier to imagine as an OrderedDict which items can be accessed by index or name. This add-on
uses BfresCollection instances however, a collection preserving the index group node information and allowing access to
entries via name or index.

All this makes it quite non-trivial to create an exporter later on, as offsets have to be satisfied after the file is
completely written. This needs some brain-storming later on as it was probably solved with C pointer maths originally.
'''

class BfresFile:
    class Header:
        INDEX_GROUP_COUNT = 12

        def __init__(self, reader):
            if reader.read_raw_string(4) != "FRES":
                raise AssertionError("Invalid FRES file header.")
            self.unknown0x04 = reader.read_byte() # 0x03 in MK8
            self.unknown0x05 = reader.read_byte() # 0x00, 0x03 or 0x04 in MK8
            self.unknown0x06 = reader.read_byte() # 0x00 in MK8
            self.unknown0x07 = reader.read_byte() # 0x01, 0x02 or 0x04 in MK8
            self.embedded_byte_order = reader.read_uint16()
            self.version = reader.read_uint16() # 0x0010 in MK8
            self.file_length = reader.read_uint32()
            self.file_alignment = reader.read_uint32()
            self.file_name_offset = BfresNameOffset(reader)
            self.string_table_length = reader.read_uint32()
            self.string_table_offset = BfresOffset(reader)
            # Read the index group offsets and counts, then load the index groups.
            self.index_group_offsets = []
            for i in range(0, self.INDEX_GROUP_COUNT):
                self.index_group_offsets.append(BfresOffset(reader))
            self.index_group_nodes = []
            for i in range(0, self.INDEX_GROUP_COUNT):
                self.index_group_nodes.append(reader.read_uint16())

    class IndexGroupType(enum.IntEnum):
        Fmdl0 = 0
        Ftex1 = 1
        Fska2 = 2
        Fshu3 = 3
        Fshu4 = 4
        Fshu5 = 5
        Ftxp6 = 6
        Fvis7 = 7
        Fvis8 = 8
        Fsha9 = 9
        Fscn10 = 10
        EmbeddedFile = 11

    def __init__(self, raw):
        # Open a big-endian binary reader on the stream.
        reader = BinaryReader(raw)
        reader.endianness = ">"
        # Read the header.
        self.header = self.Header(reader)
        # Load the typed data referenced by the specific index groups.
        self._load_index_group_contents(reader)

    def _load_index_group_contents(self, reader):
        for i in range(0, self.Header.INDEX_GROUP_COUNT):
            # If an IndexGroup is not present, the offset is 0.
            offset = self.header.index_group_offsets[i]
            if offset:
                reader.seek(offset.to_file)
                if i == self.IndexGroupType.Fmdl0:
                    self.fmdl_sections = IndexGroup(reader, lambda r: FmdlSection(r))
                # TODO: Other types
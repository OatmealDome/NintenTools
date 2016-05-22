import io
import struct
from .log import Log

class Yaz0Compression:
    @staticmethod
    def decompress(compressed):
        # Not using BinaryReader and BinaryRandom here to combat the horrible performance a bit.
        Log.write(0, "Decompressing Yaz0 file...")
        # Read the header.
        if compressed.read(4).decode("ascii") != "Yaz0":
            raise AssertionError("Invalid Yaz0 header.")
        decompressed_size = struct.unpack(">I", compressed.read(4))[0]
        compressed.seek(8, io.SEEK_CUR) # Padding
        # Use an in-memory stream and open a reader/writer on it to decompress in.
        decompressed = io.BytesIO() # obviously we cannot "pre-allocate" the memory.
        # Decompress the data.
        decompressed_bytes = 0
        while decompressed_bytes < decompressed_size:
            # Read the configuration byte of a decompression setting group, and go through each bit of it.
            group_config = compressed.read(1)[0]
            for i in range(7, -1, -1):
                # Check if the bit of the current chunk is set.
                if group_config & (1 << i) == 1 << i:
                    # Bit is set, copy 1 raw byte to the output.
                    decompressed.write(compressed.read(1))
                    decompressed_bytes += 1
                elif decompressed_bytes < decompressed_size: # This does not make sense for the last byte.
                    # Bit is not set and data copying configuration follows, either 2 or 3 bytes long.
                    data_back_seek_offset = struct.unpack(">H", compressed.read(2))[0]
                    # If the nibble of the first back seek offset byte is 0, the config is 3 bytes long.
                    nibble = data_back_seek_offset >> 12 # 1 byte (8 bits) + 1 nibble (4 bits)
                    if nibble:
                        # Nibble is not 0, and determines (size + 0x02) of bytes to read.
                        data_size = nibble + 0x02
                        # Remaining bits are the real back seek offset
                        data_back_seek_offset &= 0x0FFF
                    else:
                        # Nibble is 0, the number of bytes to read is in third byte, which is (size + 0x12).
                        data_size = compressed.read(1)[0] + 0x12
                    # Since bytes can be re-read right after they were written, write and read bytes one by one.
                    for j in range(0, data_size):
                        # Read one byte from the current back seek position.
                        decompressed.seek(-data_back_seek_offset - 1, io.SEEK_CUR)
                        read_byte = decompressed.read(1)
                        # Write the byte to the end of the memory stream.
                        decompressed.seek(0, io.SEEK_END)
                        decompressed.write(read_byte)
                        decompressed_bytes += 1
        # Seek back to the start of the in-memory stream and return it.
        decompressed.seek(0)
        return decompressed

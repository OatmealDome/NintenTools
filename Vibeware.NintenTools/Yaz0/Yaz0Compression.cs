namespace Vibeware.NintenTools.Yaz0
{
    using System.Diagnostics;
    using System.IO;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents a collection of static decompression methods for Yaz0 compressed streams.
    /// </summary>
    /// <remarks>Horribly slow with this good-boy C# way. Going to rewrite it with memory mapped files soon.</remarks>
    public static class Yaz0Compression
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Decompresses all bytes of the input Yaz0 stream into the given output stream.
        /// </summary>
        /// <param name="input">The input Yaz0 stream, positioned at the beginning of the Yaz0 header.</param>
        /// <param name="output">The output stream to write the bytes to.</param>
        public static void Decompress(Stream input, Stream output)
        {
            using (BinaryDataReader reader = new BinaryDataReader(input, true))
            using (BinaryDataWriter writer = new BinaryDataWriter(output, true))
            {
                reader.ByteOrder = ByteOrder.BigEndian;

                // Read the Yaz0 header.
                if (reader.ReadString(4) != "Yaz0")
                {
                    throw new Yaz0Exception("Invalid Yaz0 header.");
                }
                uint decompressedSize = reader.ReadUInt32();
                reader.Position += 8; // Padding

                // Decompress the data.
                while (reader.Position < input.Length)
                {
                    // Read the configuration byte of a decompression setting group, and go through each bit of it.
                    byte groupConfig = reader.ReadByte();
                    for (int i = 7; i >= 0; i--)
                    {
                        // Check if bit of the current chunk is set.
                        if ((groupConfig & (1 << i)) == (1 << i))
                        {
                            // Bit is set, copy 1 raw byte to the output.
                            writer.Write(reader.ReadByte());
                        }
                        else
                        {
                            // Bit is not set and data copying configuration follows, either 2 or 3 bytes long.
                            ushort dataBackSeekOffset = reader.ReadUInt16(); // TODO: Craps up at the end of file.
                            int dataSize;
                            // If the nibble of the first back seek offset byte is 0, the config is 3 bytes long.
                            byte nibble = (byte)(dataBackSeekOffset >> 12/*1 byte (8 bits) + 1 nibble (4 bits) */);
                            if (nibble == 0)
                            {
                                // Nibble is 0, the number of bytes to read is in third byte, which is (size + 0x12).
                                dataSize = reader.ReadByte() + 0x12;
                            }
                            else
                            {
                                // Nibble is not 0, and determines (size + 0x02) of bytes to read.
                                dataSize = nibble + 0x02;
                                // Remaining bits are the real back seek offset.
                                dataBackSeekOffset &= 0x0FFF;
                            }

                            // Seek back to the data start.
                            writer.Position -= dataBackSeekOffset + 1;
                            // Since bytes can be reread right after they were written, copy and write bytes one by one.
                            while (dataSize-- > 0)
                            {
                                byte readByte = (byte)writer.BaseStream.ReadByte();
                                using (writer.TemporarySeek(0, SeekOrigin.End))
                                {
                                    writer.Write(readByte); // TODO: Super slow, flushes after every byte to read again.
                                }
                            }
                            // Seek back to the end of the stream to continue writing there.
                            writer.Position = writer.BaseStream.Length;
                        }
                    }
                }
            }
        }
    }
}
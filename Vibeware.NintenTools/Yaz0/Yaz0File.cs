namespace Vibeware.NintenTools.Yaz0
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents a Yaz0 compressed file which can be decompressed.
    /// </summary>
    public sealed class Yaz0File
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Yaz0File"/> class from the compressed file with the given
        /// file name.
        /// </summary>
        /// <param name="fileName">The name of the compressed file.</param>
        public Yaz0File(string fileName)
        {
            FileName = fileName;

            // Read and check the header.
            using (BinaryDataReader reader = new BinaryDataReader(
                new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                reader.ByteOrder = ByteOrder.BigEndian;

                if (reader.ReadString(4) != "Yaz0")
                {
                    throw new Yaz0Exception("Invalid Yaz0 header.");
                }
                DecompressedSize = reader.ReadUInt32();
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the name of the compressed file.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of the decompressed data in bytes, as specified in the Yaz0 header. It is relied on when
        /// decompressing, and decompression stops after bytes of this amount have been written.
        /// </summary>
        public uint DecompressedSize
        {
            get;
            private set;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Decompresses the contents to the file with the given name. If the file does not exist yet, it is created,
        /// otherwise overwritten.
        /// </summary>
        /// <param name="fileName">The name of the file to write the decompressed data to.</param>
        public void Decompress(string fileName)
        {
            // Open the input file again to start reading after the header.
            using (BinaryDataReader inputReader = new BinaryDataReader(
                new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            // Map the output file into memory and create two streams to read from and write to it.
            using (MemoryMappedFile outputFile = MemoryMappedFile.CreateFromFile(fileName, FileMode.Create,
                Path.GetFileName(fileName), DecompressedSize, MemoryMappedFileAccess.ReadWrite))
            using (BinaryDataReader outputReader = new BinaryDataReader(
                outputFile.CreateViewStream(0/*offset*/, 0/*size = all*/, MemoryMappedFileAccess.Read)))
            using (BinaryDataWriter outputWriter = new BinaryDataWriter(
                outputFile.CreateViewStream(0/*offset*/, 0/*size = all*/, MemoryMappedFileAccess.Write)))
            {
                inputReader.ByteOrder = ByteOrder.BigEndian;

                // Position the input file stream after the header (char[4] + uint + padding[8]).
                inputReader.Position = 16;

                // Decompress the data.
                int decompressedBytes = 0;
                while (decompressedBytes < DecompressedSize)
                {
                    // Read the configuration byte of a decompression setting group, and go through each bit of it.
                    byte groupConfig = inputReader.ReadByte();
                    for (int i = 7; i >= 0; i--)
                    {
                        // Check if bit of the current chunk is set.
                        if ((groupConfig & (1 << i)) == (1 << i))
                        {
                            // Bit is set, copy 1 raw byte to the output.
                            outputWriter.Write(inputReader.ReadByte());
                            decompressedBytes++;
                        }
                        else
                        {
                            // Bit is not set and data copying configuration follows, either 2 or 3 bytes long.
                            ushort dataBackSeekOffset = inputReader.ReadUInt16();
                            int dataSize;
                            // If the nibble of the first back seek offset byte is 0, the config is 3 bytes long.
                            byte nibble = (byte)(dataBackSeekOffset >> 12/*1 byte (8 bits) + 1 nibble (4 bits) */);
                            if (nibble == 0)
                            {
                                // Nibble is 0, the number of bytes to read is in third byte, which is (size + 0x12).
                                dataSize = inputReader.ReadByte() + 0x12;
                            }
                            else
                            {
                                // Nibble is not 0, and determines (size + 0x02) of bytes to read.
                                dataSize = nibble + 0x02;
                                // Remaining bits are the real back seek offset.
                                dataBackSeekOffset &= 0x0FFF;
                            }
                            // Since bytes can be reread right after they were written, write and read bytes one by one.
                            outputReader.Position = outputWriter.Position - dataBackSeekOffset - 1;
                            while (dataSize-- > 0)
                            {
                                outputWriter.Write(outputReader.ReadByte());
                                decompressedBytes++;
                            }
                        }
                    }
                }
            }
        }
    }
}

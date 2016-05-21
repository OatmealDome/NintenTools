namespace Syroot.NintenTools.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents an extended <see cref="BinaryReader"/> supporting special file format data types.
    /// </summary>
    public class BinaryDataReader : BinaryReader
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDataReader"/> class based on the specified stream and
        /// using UTF-8 encoding.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <exception cref="ArgumentException">The stream does not support reading, is null, or is already closed.
        /// </exception>
        public BinaryDataReader(Stream input)
            : base(input)
        {
            Encoding = new UTF8Encoding();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDataReader"/> class based on the specified stream, UTF-8
        /// encoding and optionally leaves the stream open.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after the <see cref="BinaryDataReader"/> object
        /// is disposed; otherwise <c>false</c>.</param>
        /// <exception cref="ArgumentException">The stream does not support reading, is null, or is already closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">encoding is null.</exception>
        public BinaryDataReader(Stream input, bool leaveOpen)
            : base(input, new UTF8Encoding(), leaveOpen)
        {
            Encoding = new UTF8Encoding();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDataReader"/> class based on the specified stream and
        /// character encoding.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentException">The stream does not support reading, is null, or is already closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">encoding is null.</exception>
        public BinaryDataReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
            Encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDataReader"/> class based on the specified stream and
        /// character encoding, and optionally leaves the stream open.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after the <see cref="BinaryDataReader"/> object
        /// is disposed; otherwise <c>false</c>.</param>
        /// <exception cref="ArgumentException">The stream does not support reading, is null, or is already closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">encoding is null.</exception>
        public BinaryDataReader(Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
            Encoding = encoding;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the encoding used for string related operations where no other encoding has been provided. Due to the
        /// way the underlying <see cref="BinaryReader"/> is instantiated, it can only be specified at creation time.
        /// </summary>
        public Encoding Encoding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the byte order used to parse binary data with.
        /// </summary>
        public ByteOrder ByteOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the length in bytes of the stream in bytes. This is a shortcut to the base stream Length property.
        /// </summary>
        public long Length
        {
            get { return BaseStream.Length; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream. This is a shortcut to the base stream Position
        /// property.
        /// </summary>
        public long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the end of the stream has been reached and no more data can be read.
        /// </summary>
        public bool EndOfStream
        {
            get { return BaseStream.Position >= BaseStream.Length; }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the current stream. The <see cref="DateTime"/> is available in the
        /// specified binary format.
        /// </summary>
        /// <param name="format">The binary format, in which the <see cref="DateTime"/> will be read.</param>
        /// <returns>The <see cref="DateTime"/> read from the current stream.</returns>
        public DateTime ReadDateTime(BinaryDateTimeFormat format)
        {
            switch (format)
            {
                case BinaryDateTimeFormat.CTime:
                    return new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(ReadUInt32());
                default:
                    throw new ArgumentOutOfRangeException("format", "The specified binary datetime format is invalid");
            }
        }

        /// <summary>
        /// Reads an 8-byte floating point value from the current stream and advances the current position of the stream
        /// by eight bytes.
        /// </summary>
        /// <returns>The 8-byte floating point value read from the current stream.</returns>
        public override double ReadDouble()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(double));
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }
            else
            {
                return base.ReadDouble();
            }
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two
        /// bytes.
        /// </summary>
        /// <returns>The 2-byte signed integer read from the current stream.</returns>
        public override short ReadInt16()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(short));
                Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes, 0);
            }
            else
            {
                return base.ReadInt16();
            }
        }

        /// <summary>
        /// Reads the specified number of <see cref="Int16"/> values from the current stream into a <see cref="Int16"/>
        /// array and advances the current position by that number of <see cref="Int16"/> values multiplied with the
        /// size of a single value.
        /// </summary>
        /// <param name="count">The number of <see cref="Int16"/> values to read.</param>
        /// <returns>The <see cref="Int16"/> array containing data read from the current stream. This might be less than
        /// the number of bytes requested if the end of the stream is reached.</returns>
        public short[] ReadInt16s(int count)
        {
            short[] shorts = new short[count];
            for (int i = 0; i < shorts.Length; i++)
            {
                shorts[i] = ReadInt16();
            }
            return shorts;
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by
        /// four bytes.
        /// </summary>
        /// <returns>The 4-byte signed integer read from the current stream.</returns>
        public override int ReadInt32()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(int));
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
            else
            {
                return base.ReadInt32();
            }
        }

        /// <summary>
        /// Reads the specified number of <see cref="Int32"/> values from the current stream into a <see cref="Int32"/>
        /// array and advances the current position by that number of <see cref="Int32"/> values multiplied with the
        /// size of a single value.
        /// </summary>
        /// <param name="count">The number of <see cref="Int32"/> values to read.</param>
        /// <returns>The <see cref="Int32"/> array containing data read from the current stream. This might be less than
        /// the number of bytes requested if the end of the stream is reached.</returns>
        public int[] ReadInt32s(int count)
        {
            int[] ints = new int[count];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = ReadInt32();
            }
            return ints;
        }

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by
        /// eight bytes.
        /// </summary>
        /// <returns>The 8-byte signed integer read from the current stream.</returns>
        public override long ReadInt64()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(long));
                Array.Reverse(bytes);
                return BitConverter.ToInt64(bytes, 0);
            }
            else
            {
                return base.ReadInt64();
            }
        }

        /// <summary>
        /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream
        /// by four bytes.
        /// </summary>
        /// <returns>The 4-byte floating point value read from the current stream.</returns>
        public override float ReadSingle()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(float));
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
            else
            {
                return base.ReadSingle();
            }
        }

        /// <summary>
        /// Reads the specified number of <see cref="Single"/> values from the current stream into a
        /// <see cref="Single"/> array and advances the current position by that number of <see cref="Single"/> values
        /// multiplied with the size of a single value.
        /// </summary>
        /// <param name="count">The number of <see cref="Single"/> values to read.</param>
        /// <returns>The <see cref="Single"/> array containing data read from the current stream. This might be less
        /// than the number of bytes requested if the end of the stream is reached.</returns>
        public float[] ReadSingles(int count)
        {
            float[] floats = new float[count];
            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = ReadSingle();
            }
            return floats;
        }

        /// <summary>
        /// Reads a string from the current stream. The string is available in the specified binary format.
        /// </summary>
        /// <param name="format">The binary format, in which the string will be read.</param>
        /// <returns>The string read from the current stream.</returns>
        public string ReadString(BinaryStringFormat format)
        {
            return ReadString(format, Encoding);
        }

        /// <summary>
        /// Reads a string from the current stream. The string is available in the specified binary format and encoding.
        /// </summary>
        /// <param name="format">The binary format, in which the string will be read.</param>
        /// <param name="encoding">The encoding used for converting the string.</param>
        /// <returns>The string read from the current stream.</returns>
        public string ReadString(BinaryStringFormat format, Encoding encoding)
        {
            switch (format)
            {
                case BinaryStringFormat.ByteLengthPrefix:
                    return ReadByteLengthPrefixString(encoding);
                case BinaryStringFormat.WordLengthPrefix:
                    return ReadWordLengthPrefixString(encoding);
                case BinaryStringFormat.DwordLengthPrefix:
                    return ReadDwordLengthPrefixString(encoding);
                case BinaryStringFormat.ZeroTerminated:
                    return ReadZeroTerminatedString(encoding);
                case BinaryStringFormat.NoPrefixOrTermination:
                    throw new ArgumentException("NoPrefixOrTermination cannot be used for read operations if no length "
                        + "has been specified.", "format");
                default:
                    throw new ArgumentOutOfRangeException("format", "The specified binary string format is invalid");
            }
        }

        /// <summary>
        /// Reads a string from the current stream. The string has neither a prefix or postfix, the length has to be
        /// specified manually.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <returns>The string read from the current stream.</returns>
        public string ReadString(int length)
        {
            return ReadString(length, Encoding);
        }

        /// <summary>
        /// Reads a string from the current stream. The string has neither a prefix or postfix, the length has to be
        /// specified manually. The string is available in the specified encoding.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <param name="encoding">The encoding to use for reading the string.</param>
        /// <returns>The string read from the current stream.</returns>
        public string ReadString(int length, Encoding encoding)
        {
            return encoding.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the
        /// position of the stream by two bytes.
        /// </summary>
        /// <returns>The 2-byte unsigned integer read from the current stream.</returns>
        public override ushort ReadUInt16()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(ushort));
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
            else
            {
                return base.ReadUInt16();
            }
        }

        /// <summary>
        /// Reads the specified number of <see cref="UInt16"/> values from the current stream into a
        /// <see cref="UInt16"/> array and advances the current position by that number of <see cref="UInt16"/> values
        /// multiplied with the size of a single value.
        /// </summary>
        /// <param name="count">The number of <see cref="UInt16"/> values to read.</param>
        /// <returns>The <see cref="UInt16"/> array containing data read from the current stream. This might be less
        /// than the number of bytes requested if the end of the stream is reached.</returns>
        public ushort[] ReadUInt16s(int count)
        {
            ushort[] ushorts = new ushort[count];
            for (int i = 0; i < ushorts.Length; i++)
            {
                ushorts[i] = ReadUInt16();
            }
            return ushorts;
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight
        /// bytes.
        /// </summary>
        /// <returns>The 8-byte unsigned integer read from the current stream.</returns>
        public override uint ReadUInt32()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(uint));
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
            else
            {
                return base.ReadUInt32();
            }
        }

        /// <summary>
        /// Reads the specified number of <see cref="UInt32"/> values from the current stream into a
        /// <see cref="UInt32"/> array and advances the current position by that number of <see cref="UInt32"/> values
        /// multiplied with the size of a single value.
        /// </summary>
        /// <param name="count">The number of <see cref="UInt32"/> values to read.</param>
        /// <returns>The <see cref="UInt32"/> array containing data read from the current stream. This might be less
        /// than the number of bytes requested if the end of the stream is reached.</returns>
        public uint[] ReadUInt32s(int count)
        {
            uint[] uints = new uint[count];
            for (uint i = 0; i < uints.Length; i++)
            {
                uints[i] = ReadUInt32();
            }
            return uints;
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight
        /// bytes.
        /// </summary>
        /// <returns>The 8-byte unsigned integer read from the current stream.</returns>
        public override ulong ReadUInt64()
        {
            if (ByteOrder == ByteOrder.BigEndian)
            {
                byte[] bytes = base.ReadBytes(sizeof(ulong));
                Array.Reverse(bytes);
                return BitConverter.ToUInt64(bytes, 0);
            }
            else
            {
                return base.ReadUInt64();
            }
        }

        /// <summary>
        /// Sets the position within the current stream. This is a shortcut to the base stream Seek method.
        /// </summary>
        /// <param name="offset">A byte offset relative to the current position in the stream.</param>
        /// <returns>The new position within the current stream.</returns>
        public long Seek(long offset)
        {
            return Seek(offset, SeekOrigin.Current);
        }
        
        /// <summary>
        /// Sets the position within the current stream. This is a shortcut to the base stream Seek method.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain
        /// the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Creates a <see cref="SeekTask"/> to restore the current position after it has been disposed.
        /// </summary>
        /// <returns>The <see cref="SeekTask"/> to be disposed to restore to the current position.</returns>
        public SeekTask TemporarySeek()
        {
            return TemporarySeek(0, SeekOrigin.Current);
        }

        /// <summary>
        /// Creates a <see cref="SeekTask"/> with the given parameters. As soon as the returned <see cref="SeekTask"/>
        /// is disposed, the previous stream position will be restored.
        /// </summary>
        /// <param name="offset">A byte offset relative to the current position in the stream.</param>
        /// <returns>The <see cref="SeekTask"/> to be disposed to undo the seek.</returns>
        public SeekTask TemporarySeek(long offset)
        {
            return TemporarySeek(offset, SeekOrigin.Current);
        }

        /// <summary>
        /// Creates a <see cref="SeekTask"/> with the given parameters. As soon as the returned <see cref="SeekTask"/>
        /// is disposed, the previous stream position will be restored.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain
        /// the new position.</param>
        /// <returns>The <see cref="SeekTask"/> to be disposed to undo the seek.</returns>
        public SeekTask TemporarySeek(long offset, SeekOrigin origin)
        {
            return new SeekTask(BaseStream, offset, origin);
        }
        
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private string ReadByteLengthPrefixString(Encoding encoding)
        {
            int length = ReadByte();

            // This will not work for strings with differently sized characters depending on their code.
            int charSize = encoding.GetByteCount("a");

            return encoding.GetString(ReadBytes(length * charSize));
        }

        private string ReadWordLengthPrefixString(Encoding encoding)
        {
            int length = ReadInt16();

            // This will not work for strings with differently sized characters depending on their code.
            int charSize = encoding.GetByteCount("a");

            return encoding.GetString(ReadBytes(length * charSize));
        }

        private string ReadDwordLengthPrefixString(Encoding encoding)
        {
            int length = ReadInt32();

            // This will not work for strings with differently sized characters depending on their code.
            int charSize = encoding.GetByteCount("a");

            return encoding.GetString(ReadBytes(length * charSize));
        }

        private string ReadZeroTerminatedString(Encoding encoding)
        {
            // Read single bytes.
            List<byte> bytes = new List<byte>();
            byte readByte = ReadByte();
            while (readByte != 0)
            {
                bytes.Add(readByte);
                readByte = ReadByte();
            }

            // Convert to string.
            return encoding.GetString(bytes.ToArray());
        }
    }
}

namespace Vibeware.NintenTools.IO
{
    /// <summary>
    /// Represents the possible endianness of binary data.
    /// </summary>
    public enum ByteOrder : ushort
    {
        /// <summary>
        /// The binary data is present in big endian.
        /// </summary>
        BigEndian = 0xFEFF,

        /// <summary>
        /// The binary data is present in little endian.
        /// </summary>
        LittleEndian = 0xFFFE
    }
}

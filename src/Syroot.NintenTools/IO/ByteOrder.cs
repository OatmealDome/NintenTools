using System;

namespace Syroot.NintenTools.IO
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

    /// <summary>
    /// Represents extension methods for the <see cref="ByteOrder"/> enumeration.
    /// </summary>
    public static class ByteOrderExtensions
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private static ByteOrder _systemByteOrder;

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="ByteOrder"/> of the system executing the assembly.
        /// </summary>
        /// <returns></returns>
        public static ByteOrder GetSystemByteOrder(this ByteOrder byteOrder)
        {
            if (_systemByteOrder == 0)
            {
                // The BitConverter.IsLittleEndian field is only set after its static constructor has been run.
                BitConverter.GetBytes(0);
                _systemByteOrder = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            }
            return _systemByteOrder;
        }
    }
}

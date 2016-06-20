namespace Syroot.NintenTools.Bfres
{
    using System.Diagnostics;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents an offset in a BFRES file. As offset values in a BFRES file are relative to themselves, this struct
    /// provides an easy way to compute the total offset relative to the beginning of the file.
    /// </summary>
    [DebuggerDisplay("{ToSelf}")]
    public struct BfresOffset
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// The address at which the offset represented by <see cref="ToSelf"/> is stored.
        /// </summary>
        public readonly uint Address;

        /// <summary>
        /// The offset relative to the offsets value itself (can be negative).
        /// </summary>
        public readonly int ToSelf;

        /// <summary>
        /// The total offset relative to the beginning of the BFRES file.
        /// </summary>
        public readonly uint ToFile;

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresOffset"/> struct from the given
        /// <see cref="BinaryDataReader"/>, which base streams position is advanced by 4 bytes.
        /// </summary>
        internal BfresOffset(BinaryDataReader reader)
        {
            Address = (uint)reader.Position;
            ToSelf = reader.ReadInt32();
            ToFile = (uint)(Address + ToSelf);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether this offset is not set and zero.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ToSelf == 0;
            }
        }
    }
}

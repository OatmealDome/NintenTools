namespace Syroot.NintenTools.Bfres
{
    using System.Diagnostics;
    using System.IO;
    using Syroot.IO;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents an offset in a BFRES file pointing to a string in the string table. As offset values in a BFRES file
    /// are relative to themselves, this struct provides an easy way to compute the total offset relative to the
    /// beginning of the file.
    /// </summary>
    [DebuggerDisplay("{ToSelf}, {Name}")]
    public struct BfresNameOffset
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

        /// <summary>
        /// The name to which the offset points to.
        /// </summary>
        public readonly string Name;

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresNameOffset"/> struct from the given
        /// <see cref="BinaryDataReader"/>, which base streams position is advanced by 4 bytes. A temporary seek is
        /// performed to read in the referenced name.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryDataReader"/> to read the data from.</param>
        internal BfresNameOffset(BinaryDataReader reader)
        {
            Address = (uint)reader.Position;
            ToSelf = reader.ReadInt32();
            ToFile = (uint)(Address + ToSelf);

            // Strings are DWORD length-prefixed zero-postfixed, we decide to read a zero-terminated string here.
            // To read the length-prefix, you would have to seek 4 bytes back, as the offsets point to the first char.
            using (reader.TemporarySeek(ToFile, SeekOrigin.Begin))
            {
                Name = reader.ReadString(BinaryStringFormat.ZeroTerminated);
            }
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

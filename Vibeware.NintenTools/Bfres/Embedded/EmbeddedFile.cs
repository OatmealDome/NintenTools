namespace Vibeware.NintenTools.Bfres.Embedded
{
    /// <summary>
    /// Represents an embedded file in a BFRES file.
    /// </summary>
    public class EmbeddedFile
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFile"/> for the given <see cref="BfresFile"/>.
        /// The stream of the file has to be positioned at the beginning of the data.
        /// </summary>
        /// <param name="bfresFile">The BFRES file providing the context.</param>
        internal EmbeddedFile(BfresFile bfresFile)
        {
        }
    }
}
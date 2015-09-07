namespace Vibeware.NintenTools.Bfres.Fvis
{
    /// <summary>
    /// Represents an FVIS section in a BFRES file which contains bone visibility.
    /// </summary>
    public class FvisSection
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FvisSection"/> for the given <see cref="BfresFile"/>.
        /// The stream of the file has to be positioned at the beginning of the data.
        /// </summary>
        /// <param name="bfresFile">The BFRES file providing the context.</param>
        internal FvisSection(BfresFile bfresFile)
        {
        }
    }
}
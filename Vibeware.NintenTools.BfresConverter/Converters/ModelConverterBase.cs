namespace Vibeware.NintenTools.BfresConverter.Converters
{
    using System.IO;
    using Vibeware.NintenTools.Bfres;

    /// <summary>
    /// Represents the common methods implemented by classes converting a BFRES model into another format.
    /// </summary>
    internal abstract class ModelConverterBase
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConverterBase"/> class with the given
        /// <see cref="BfresFile"/> and output <see cref="Stream"/>.
        /// </summary>
        /// <param name="bfresFile">The <see cref="BfresFile"/> containing the model data.</param>
        /// <param name="output">The output <see cref="Stream"/>.</param>
        internal ModelConverterBase(BfresFile bfresFile, FileStream output)
        {
            BfresFile = bfresFile;
            Output = output;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the input BFRES file containing the model data to convert.
        /// </summary>
        protected BfresFile BfresFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the output stream to write the converted model data to.
        /// </summary>
        protected FileStream Output
        {
            get;
            private set;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Converts the BFRES file into the model format.
        /// </summary>
        internal abstract void Convert();
    }
}
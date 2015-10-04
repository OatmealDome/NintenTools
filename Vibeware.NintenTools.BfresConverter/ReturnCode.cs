namespace Vibeware.NintenTools.BfresConverter
{
    using System;

    /// <summary>
    /// Represents the possible return codes to return from the main program.
    /// </summary>
    public enum ReturnCode
    {
        /// <summary>
        /// Success.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// The parameter count was not 2 - in this case, the help has been printed.
        /// </summary>
        MismatchingParameterCount,

        /// <summary>
        /// The specified input file does not exist.
        /// </summary>
        InputFileDoesNotExist,

        /// <summary>
        /// The type of the input file is not supported.
        /// </summary>
        InputTypeUnsupported,

        /// <summary>
        /// The type of the output file is not supported.
        /// </summary>
        OutputTypeUnsupported,

        /// <summary>
        /// The input file cannot be opened for read access.
        /// </summary>
        InputFileNotReadable,

        /// <summary>
        /// The output file cannot be opened for write access.
        /// </summary>
        OutputFileNotWritable,

        /// <summary>
        /// The writing of the output BFRES file failed.
        /// </summary>
        OutputAsBfresFailed,

        /// <summary>
        /// The decompression of the SZS input file failed.
        /// </summary>
        InputDecompressionFailed,

        /// <summary>
        /// The input BFRES data is invalid.
        /// </summary>
        InputBfresDataInvalid,

        /// <summary>
        /// The conversion to the model format failed.
        /// </summary>
        OutputConversionFailed,

        // TODO: Get rid of this, of course.
        /// <summary>
        /// The feature has not yet been implemented.
        /// </summary>
        NotImplemented
    }

    /// <summary>
    /// Represents extension methods for the <see cref="ReturnCode"/> enumeration.
    /// </summary>
    public static class ReturnCodeExtensions
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private static readonly string[] _defaultMessage = new string[] {
            "No error",
            string.Format("Converts a BFRES model into another common model file format."
                + "{0}"
                + "{0}BFRESCONVERTER Input Output"
                + "{0}"
                + "{0}        Input   The name of the BFRES or SZS file to convert."
                + "{0}                SZS files are decompressed in memory before conversion."
                + "{0}        Output  The name of the output model file."
                + "{0}                The file extensions map to the following supported formats:"
                + "{0}                BFRES  Do not convert, just decompress SZS to BFRES"
                + "{0}                OBJ    OBJ/MTL model format with material file", Environment.NewLine),
            "The specified input file does not exist",
            "The type of the input file is not supported",
            "The type of the output file is not supported",
            "The input file cannot be opened for read access",
            "The output file cannot be opened for write access",
            "The writing of the output BFRES file failed",
            "The decompression of the SZS input file failed",
            "The input BFRES data is invalid",
            "The conversion to the model format failed",
            "The feature has not yet been implemented"
        };

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a default, descriptive message describing the return code.
        /// </summary>
        /// <param name="returnCode">The extended <see cref="ReturnCode"/> instance.</param>
        /// <returns>A message describing the return code.</returns>
        public static string GetDefaultMessage(this ReturnCode returnCode)
        {
            return _defaultMessage[(int)returnCode];
        }
    }
}

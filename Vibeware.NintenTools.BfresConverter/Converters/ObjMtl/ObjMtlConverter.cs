namespace Vibeware.NintenTools.BfresConverter.Converters.ObjMtl
{
    using System.IO;
    using Vibeware.NintenTools.Bfres;

    /// <summary>
    /// Represents a model converter supporting the OBJ/MTL format.
    /// </summary>
    internal class ObjMtlConverter : ModelConverterBase
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjMtlConverter"/> class with the given <see cref="BfresFile"/>
        ///  and output <see cref="Stream"/>.
        /// </summary>
        /// <param name="bfresFile">The <see cref="BfresFile"/> containing the model data.</param>
        /// <param name="output">The output <see cref="Stream"/>.</param>
        internal ObjMtlConverter(BfresFile bfresFile, FileStream output)
            : base(bfresFile, output)
        {
        }
        
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Converts the BFRES file into the model format.
        /// </summary>
        internal override void Convert()
        {
            throw new ConverterException(ReturnCode.NotImplemented);
        }
    }
}
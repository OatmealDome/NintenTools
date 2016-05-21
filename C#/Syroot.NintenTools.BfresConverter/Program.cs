namespace Syroot.NintenTools.BfresConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Syroot.NintenTools.Bfres;
    using Syroot.NintenTools.Yaz0;
    using Syroot.NintenTools.BfresConverter.Converters;
    using Syroot.NintenTools.BfresConverter.Converters.ObjMtl;

    /// <summary>
    /// The main class of the application.
    /// </summary>
    public class Program
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private static string     _inputFile;
        private static string     _outputFile;
        private static InputType  _inputType;
        private static OutputType _outputType;

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static int Main(string[] args)
        {
#if DEBUG
            args = new string[] { @"D:\Pictures\Iggy.bfres", @"D:\Pictures\Iggy.obj" };
            //args = new string[] { @"D:\Pictures\ClBattleShipS.bfres", @"D:\Pictures\ClBattleShipS.obj" };
#endif
            try
            {
                CheckParameters(args);
                HandleParameters();
            }
            catch (ConverterException ex)
            {
                // Print the error message for the resulting return code and return it.
                Console.WriteLine(ex.Message);
#if DEBUG
                Console.ReadLine();
#endif
                return (int)ex.ReturnCode;
            }
            
            // If you get here, everything worked alright. At least program flow says that.
            Console.WriteLine("Done");
            return (int)ReturnCode.NoError;
        }

        private static void CheckParameters(string[] args)
        {
            // At least two arguments are required, otherwise print the help.
            if (args.Length != 2)
            {
                throw new ConverterException(ReturnCode.MismatchingParameterCount);
            }
            _inputFile = args[0];
            _outputFile = args[1];
            
            // Check if the input file exists.
            if (!File.Exists(_inputFile))
            {
                throw new ConverterException(ReturnCode.InputFileDoesNotExist);
            }

            // Check if input file has either BFRES or SZS extension.
            _inputType = GetInputType(Path.GetExtension(_inputFile));
            if (_inputType == InputType.Unsupported)
            {
                throw new ConverterException(ReturnCode.InputTypeUnsupported);
            }

            // Check if output file has known model extension.
            _outputType = GetOutputType(Path.GetExtension(_outputFile));
            if (_outputType == OutputType.Unsupported)
            {
                throw new ConverterException(ReturnCode.OutputTypeUnsupported);
            }
        }

        private static void HandleParameters()
        {
            // Try to open the input and output file, and get an assured decompressed BFRES stream.
            using (FileStream input = GetInputStream())
            using (FileStream output = GetOutputStream())
            using (Stream bfresStream = _inputType == InputType.Szs ? DecompressSzsStream(input) : input)
            {
                bfresStream.Position = 0;

                // Just do a copy if the output type is BFRES.
                // TODO: Of course, if we got saving / modification features later, we can simply re-convert to BFRES.
                //       That will be a nice test to see if the re-converted file has the same output as the input.
                if (_outputType == OutputType.Bfres)
                {
                    WriteBfresFile(bfresStream, output);
                }
                else
                {
                    // Otherwise, load the BFRES model, and convert to that file format with it.
                    BfresFile bfresFile = LoadBfresFile(bfresStream);
                    // TODO: We could dispose the input stream now, if we wouldn't be lazy.
                    ConvertModel(bfresFile, output);
                }
            }
        }

        // ---- Helper methods -----

        private static InputType GetInputType(string extension)
        {
            switch (extension.ToUpperInvariant())
            {
                case ".BFRES":
                    return InputType.Bfres;
                case ".SZS":
                    return InputType.Szs;
                default:
                    return InputType.Unsupported;
            }
        }

        private static OutputType GetOutputType(string extension)
        {
            switch (extension.ToUpperInvariant())
            {
                case ".BFRES":
                    return OutputType.Bfres;
                case ".OBJ":
                    return OutputType.ObjMtl;
                default:
                    return OutputType.Unsupported;
            }
        }

        private static FileStream GetInputStream()
        {
            FileStream input;
            try
            {
                input = new FileStream(_inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                throw new ConverterException(ReturnCode.InputFileNotReadable);
            }

            return input;
        }

        private static FileStream GetOutputStream()
        {
            FileStream output;
            try
            {
                output = new FileStream(_outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            catch (Exception)
            {
                throw new ConverterException(ReturnCode.OutputFileNotWritable);
            }

            return output;
        }

        private static Stream DecompressSzsStream(Stream input)
        {
            // Decompress an SZS input file into memory and return the memory stream.
            Console.Write("Decompressing SZS file... ");
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                Yaz0Compression.Decompress(input, memoryStream);
                Console.WriteLine("Success");
                return memoryStream;
            }
            catch (Yaz0Exception ex)
            {
                // Dispose the memory stream here, as it will not be returned.
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
                throw new ConverterException(ReturnCode.InputDecompressionFailed, ex.Message);
            }
        }

        private static void WriteBfresFile(Stream bfresStream, Stream output)
        {
            // Simply copy the input BFRES data to the output.
            Console.Write("Writing BFRES file... ");
            try
            {
                bfresStream.CopyTo(output);
                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                throw new ConverterException(ReturnCode.OutputAsBfresFailed, ex.Message);
            }
        }

        private static BfresFile LoadBfresFile(Stream bfresStream)
        {
            BfresFile bfresFile = new BfresFile();
            List<string> warnings;

            // Try loading the BFRES file.
            Console.Write("Loading BFRES data... ");
            try
            {
                warnings = bfresFile.Load(bfresStream);
            }
            catch (BfresException ex)
            {
                throw new ConverterException(ReturnCode.InputBfresDataInvalid, ex.Message);
            }

            // Output warnings to console.
            if (warnings.Count == 0)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("{0} warning{1} occured:", warnings.Count, warnings.Count == 1 ? String.Empty : "s");
                foreach (string warning in warnings)
                {
                    Console.WriteLine("\t" + warning);
                }
            }

            return bfresFile;
        }

        private static void ConvertModel(BfresFile bfresFile, FileStream output)
        {
            // Try converting the BFRES model.
            Console.Write("Converting model... ");
            try
            {
                ModelConverterBase modelConverter = GetModelConverter(_outputType, bfresFile, output);
                modelConverter.Convert();
                Console.WriteLine("Success");
            }
            catch (ConverterException ex)
            {
                throw new ConverterException(ReturnCode.OutputConversionFailed, ex.Message);
            }
        }

        private static ModelConverterBase GetModelConverter(OutputType outputType, BfresFile bfresFile,
            FileStream output)
        {
            switch (outputType)
            {
                case OutputType.ObjMtl:
                    return new ObjMtlConverter(bfresFile, output);
                default:
                    throw new ConverterException(ReturnCode.OutputTypeUnsupported);
            }
        }

        // ---- ENUMERATIONS -------------------------------------------------------------------------------------------

        private enum InputType
        {
            Unsupported = 0,
            Bfres,
            Szs
        }

        private enum OutputType
        {
            Unsupported = 0,
            Bfres,
            ObjMtl
        }
    }
}

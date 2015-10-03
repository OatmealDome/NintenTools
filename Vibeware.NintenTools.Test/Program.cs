namespace Vibeware.NintenTools.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Vibeware.NintenTools.Bfres;
    using Vibeware.NintenTools.Yaz0;

    /// <summary>
    /// The main class of the application.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private static void Main(string[] args)
        {
            DecompressYaz0File(@"D:\Pictures\Iggy.szs", @"D:\Pictures\Iggy.bfres");
            LoadBfresFile(@"D:\Pictures\Iggy.bfres");
        }

        private static void LoadBfresFile(string fileName)
        {
            Console.Write("Loading \"{0}\"... ", fileName);

            BfresFile bfresFile;
            List<string> warnings;

            // Load the given BFRES file.
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bfresFile = new BfresFile();
                warnings = bfresFile.Load(stream);
            }

            // Output warnings to console.
            Console.WriteLine("{0} warnings occured when loading the file.", warnings.Count);
            foreach (string warning in warnings)
            {
                Console.WriteLine(warning);
            }
            Console.ReadLine();
        }

        private static void DecompressYaz0File(string inputFileName, string outputFileName)
        {
            Console.Write("Decompressing \"{0}\" to \"{1}\"... ", inputFileName, outputFileName);

            Yaz0File yaz0File = new Yaz0File(inputFileName);
            yaz0File.Decompress(outputFileName);

            Console.WriteLine("successfully decompressed.");
        }
    }
}

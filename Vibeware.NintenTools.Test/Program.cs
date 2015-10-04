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
            // Decompress an example file into a memory stream.
            using (MemoryStream bfresDataStream = new MemoryStream())
            {
                DecompressYaz0File(@"D:\Pictures\Iggy.szs", bfresDataStream);
                // Load the decompressed BFRES contents from the data stream.
                LoadBfresFile(bfresDataStream);
            }

            Console.ReadLine();
        }

        private static void DecompressYaz0File(string inputFile, MemoryStream output)
        {
            Console.Write("Decompressing \"{0}\"... ", inputFile);

            int bytesDecompressed;
            using (FileStream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bytesDecompressed = Yaz0Compression.Decompress(input, output);
            }
            
            Console.WriteLine("successfully decompressed {0} bytes.", bytesDecompressed);
        }

        private static void LoadBfresFile(MemoryStream dataStream)
        {
            Console.Write("Loading BFRES data... ");
            
            // Load the BFRES data from the start of the given stream.
            BfresFile bfresFile = new BfresFile();
            dataStream.Position = 0;
            List<string> warnings = bfresFile.Load(dataStream);

            // Output warnings to console.
            Console.WriteLine("{0} warnings occured when loading the BFRES data.", warnings.Count);
            foreach (string warning in warnings)
            {
                Console.WriteLine(warning);
            }
        }
    }
}

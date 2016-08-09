using System;
using System.Collections.Generic;
using System.IO;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Byaml;
using Syroot.NintenTools.Yaz0;

namespace Syroot.NintenTools.Test
{
    /// <summary>
    /// The main class of the application.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private static void Main(string[] args)
        {
            TestByaml();
        }

        // ---- BFRES ----

        private static void TestBfres()
        {
            using (FileStream fileStream = new FileStream(@"D:\Pictures\Blender Work Folder\Iggy.bfres", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                LoadBfresFile(fileStream);
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

        private static void LoadBfresFile(Stream dataStream)
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

        // ---- BYAML ----

        private static void TestByaml()
        {
            ByamlNode byaml = ByamlNode.Load(@"D:\Archive\Games\Emulators\Wii U\Roms\Mario Kart 8 EUR 4.1 with DLC1+2\content\course\Gu_FirstCircuit\course_muunt.byaml");
        }
    }
}

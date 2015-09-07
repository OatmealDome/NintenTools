namespace Vibeware.NintenTools.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Vibeware.NintenTools.Bfres;

    /// <summary>
    /// The main class of the application.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private static void Main(string[] args)
        {
            BfresFile bfresFile;
            List<string> warnings;

            // Load an example BFRES file.
            using (FileStream stream = new FileStream(@"D:\Pictures\Iggy.bfres", FileMode.Open, FileAccess.Read,
                FileShare.Read))
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
    }
}

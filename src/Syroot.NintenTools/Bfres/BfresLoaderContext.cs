namespace Syroot.NintenTools.Bfres
{
    using System.Collections.Generic;
    using Syroot.IO;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents a set of properties controlling the load of a <see cref="Bfres.BfresFile"/>.
    /// </summary>
    internal class BfresLoaderContext
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresLoaderContext"/> class for the given <see cref="BfresFile"/>
        /// instance using the given <see cref="BinaryDataReader"/>.
        /// </summary>
        /// <param name="bfresFile">The <see cref="BfresFile"/> instance to load the data in.</param>
        /// <param name="reader">The <see cref="BinaryDataReader"/> to use for reading the data.</param>
        internal BfresLoaderContext(BfresFile bfresFile, BinaryDataReader reader)
        {
            BfresFile = bfresFile;
            Reader = reader;
            Warnings = new List<string>();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="BfresFile"/> instance which is loaded to.
        /// </summary>
        internal BfresFile BfresFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="BinaryDataReader"/> which is used to read data from the input stream.
        /// </summary>
        internal BinaryDataReader Reader
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the warnings raised after loading the BFRES file.
        /// </summary>
        internal List<string> Warnings
        {
            get;
            private set;
        }
    }
}

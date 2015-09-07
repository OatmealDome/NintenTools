using System;

namespace Vibeware.NintenTools.Bfres.Fmdl
{
    /// <summary>
    /// Represents a slice of index buffer data which draws a geometry part of a <see cref="FshpLodModel"/>.
    /// </summary>
    public class FshpVisibilityGroup
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FshpVisibilityGroup"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FshpVisibilityGroup(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the offset into the index buffer in bytes.
        /// </summary>
        public uint Offset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of indices to read.
        /// </summary>
        public uint Count
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private void Load(BfresLoaderContext context)
        {
            Offset = context.Reader.ReadUInt32();
            Count = context.Reader.ReadUInt32();
        }
    }
}
namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents a generic parameter referenced by a FMDL section which describes arbitrary model parameters.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class FmdlParameter
    {
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmdlSection"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the name in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 1.
            /// </summary>
            public ushort Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x06
            {
                get;
                set;
            }
        }

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmdlParameter"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmdlParameter(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
            /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmdlParameter"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name to describe the parameter.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public float Value
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            Internal.Unknown0x04 = context.Reader.ReadUInt16();
            Internal.Unknown0x06 = context.Reader.ReadUInt16();
            Value = context.Reader.ReadSingle();
        }
    }
}
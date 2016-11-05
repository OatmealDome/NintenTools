using System.Diagnostics;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Bfres.Fmdl
{
    /// <summary>
    /// Represents an shadow parameter in an FMAT subsection which describes the value to assign to a uniform shadow
    /// variable in the shader.
    /// </summary>
    [DebuggerDisplay("Shadow Parameter {Name}")]
    public class FmatShadowParameter
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatShadowParameter"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatShadowParameter(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatShadowParameter"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the shadow parameter uniform variable.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the shadow parameter uniform variable.
        /// </summary>
        public uint Value
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
            if (Internal.Unknown0x04 != 1)
            {
                context.Warnings.Add("FmatMaterial.Unknown0x04 has unexpected value of " + Internal.Unknown0x04);
            }

            Internal.Unknown0x06 = context.Reader.ReadByte();

            Internal.Unknown0x07 = context.Reader.ReadByte();
            if (Internal.Unknown0x07 != 0)
            {
                context.Warnings.Add("FmatMaterial.Unknown0x07 has unexpected value of " + Internal.Unknown0x07);
            }

            Value = context.Reader.ReadUInt32();
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatShadowParameter"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the name of the variable in the string table.
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
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be a type or index.
            /// </summary>
            public byte Unknown0x06
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x07. The value always seems to be 0.
            /// </summary>
            public byte Unknown0x07
            {
                get;
                set;
            }
        }
    }
}
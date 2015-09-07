namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System;
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;
    using Vibeware.NintenTools.Maths;

    /// <summary>
    /// Represents a render parameter in an FMAT subsection, describing the value to assign to a uniform variable in
    /// shader code.
    /// </summary>
    [DebuggerDisplay("{Type} {Name} = {Value}")]
    public class FmatRenderParameter
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatRenderParameter"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatRenderParameter(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatRenderParameter"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the variable. To get a correctly typed variable, use one of the Get methods.
        /// </summary>
        public FmatRenderParameterType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the variable name in shader code.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the variable value. Depending on <see cref="Type"/>, this is of different type.
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x00 != 0 && Internal.Unknown0x00 != 1)
            {
                context.Warnings.Add("FmatRenderParameter.Unknown0x00 has unexpected value of " + Internal.Unknown0x00);
            }

            Type = (FmatRenderParameterType)context.Reader.ReadByte();
            if (!Enum.IsDefined(typeof(FmatRenderParameterType), Type))
            {
                throw new BfresException("FmatRenderParameter.Type invalid");
            }

            Internal.Unknown0x03 = context.Reader.ReadByte();
            if (Internal.Unknown0x03 != 0)
            {
                context.Warnings.Add("FmatRenderParameter.Unknown0x03 has unexpected value of " + Internal.Unknown0x03);
            }

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            
            switch (Type)
            {
                case FmatRenderParameterType.Null8Bytes:
                    Value = context.Reader.ReadBytes(8);
                    break;
                case FmatRenderParameterType.Vector2F:
                    Value = context.Reader.ReadVector2F();
                    break;
                case FmatRenderParameterType.StringOffset:
                    // Let's store the name instead of the BfresNameOffset for now. No idea if someone ever wants to
                    // check back on internal values anyway...
                    Value = context.Reader.ReadBfresNameOffset().Name;
                    break;
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatRenderParameter"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x00. The value always seems to be 0x00 or 0x01.
            /// </summary>
            public ushort Unknown0x00
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x03. The value always seems to be 0x00.
            /// </summary>
            public byte Unknown0x03
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the parameter's <see cref="Name"/> in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }
        }
    }

    /// <summary>
    /// Represents the possible types an <see cref="FmatRenderParameter"/> can reference.
    /// </summary>
    public enum FmatRenderParameterType : byte
    {
        /// <summary>
        /// Probably represents an array of 8 zero bytes.
        /// </summary>
        Null8Bytes = 0x00,

        /// <summary>
        /// Probably represents 2 Vector2F instances.
        /// </summary>
        Vector2F = 0x01,

        /// <summary>
        /// Represents the offset to a string.
        /// </summary>
        StringOffset = 0x02
    }
}
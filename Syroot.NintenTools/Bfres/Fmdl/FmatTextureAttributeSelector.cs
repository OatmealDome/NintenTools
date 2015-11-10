namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System.Diagnostics;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents an attribute in an FMAT subsection, describing how a texture is passed to the pixel shader input.
    /// </summary>
    [DebuggerDisplay("Texture Attribute Selector {Name}")]
    public class FmatTextureAttributeSelector
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatTextureAttributeSelector"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatTextureAttributeSelector(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatTextureAttributeSelector"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        /// <remarks>
        /// The following naming convention seems to be used for attributes:
        /// _a0  albedo0    Diffuse color
        /// _a1  albedo1    Diffuse color
        /// _a2  albedo2    Diffuse color
        /// _a3  albedo3    Diffuse color
        /// _s0  specular0  Specular lighting color
        /// _n0  normal0    Normal map
        /// _n1  normal1    Normal map
        /// _e0  emission0  Emissive lighting
        /// _b0  bake0      Shadow textures
        /// _b1  bake1      Shadow textures
        /// </remarks>
        public string Name
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadByte();
            if (Internal.Unknown0x00 != 2)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x00 has unexpected value of " + Internal.Unknown0x00);
            }

            Internal.Unknown0x01 = context.Reader.ReadByte();
            if (Internal.Unknown0x01 != 0x00 && Internal.Unknown0x01 != 0x02 && Internal.Unknown0x01 != 0x04
                && Internal.Unknown0x01 != 0x12)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x01 has unexpected value of " + Internal.Unknown0x01);
            }

            Internal.Unknown0x02 = context.Reader.ReadByte();
            if (Internal.Unknown0x02 != 0x00 && Internal.Unknown0x02 != 0x10 && Internal.Unknown0x02 != 0x12
                && Internal.Unknown0x02 != 0x5A)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x02 has unexpected value of " + Internal.Unknown0x02);
            }

            Internal.Unknown0x03 = context.Reader.ReadByte();
            Internal.Unknown0x04 = context.Reader.ReadSByte();
            Internal.Unknown0x05 = context.Reader.ReadByte();
            Internal.Unknown0x06 = context.Reader.ReadUInt16();

            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x08 != 0x80000000)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x08 has unexpected value of " + Internal.Unknown0x08);
            }

            Internal.Unknown0x0C = context.Reader.ReadUInt32();
            if (Internal.Unknown0x0C != 0)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x0C has unexpected value of " + Internal.Unknown0x0C);
            }

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            Internal.Index = context.Reader.ReadByte();

            Internal.Unknown0x15 = context.Reader.ReadByte();
            if (Internal.Unknown0x15 != 0)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x15 has unexpected value of " + Internal.Unknown0x15);
            }

            Internal.Unknown0x16 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x16 != 0)
            {
                context.Warnings.Add("FmatAttribute.Unknown0x16 has unexpected value of " + Internal.Unknown0x16);
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatTextureAttributeSelector"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x00. The value always seems to be 0x02.
            /// </summary>
            public byte Unknown0x00
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x01. The value always seems to be 0x00, 0x02, 0x04 or 0x12.
            /// </summary>
            public byte Unknown0x01
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x02. The value always seems to be 0x00, 0x10, 0x12 or 0x5A.
            /// </summary>
            public byte Unknown0x02
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x03. The value always seems to be near 0x00 or 0x80.
            /// </summary>
            public byte Unknown0x03
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be a signed integer close to 0.
            /// </summary>
            public sbyte Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x05. The value always seems to be small.
            /// </summary>
            public byte Unknown0x05
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be flags.
            /// </summary>
            public ushort Unknown0x06
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 0x80000000.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0C. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x0C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset the attribute's name in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of this attribute in the parent's attribute array.
            /// </summary>
            public byte Index
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x15. The value always seems to be 0.
            /// </summary>
            public byte Unknown0x15
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x16. The value always seems to be 0.
            /// </summary>
            public ushort Unknown0x16
            {
                get;
                set;
            }
        }
    }
}
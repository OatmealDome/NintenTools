using System;

namespace Vibeware.NintenTools.Bfres.Fmdl
{
    /// <summary>
    /// Represents the material data in an FMAT subsection with unknown purpose.
    /// </summary>
    public class FmatMaterialStructure
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatMaterialStructure"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatMaterialStructure(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatMaterialStructure"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Unknown0x00 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x00 >= 0x14)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x00 has unexpected value of "
                    + Internal.Unknown0x00);
            }

            Internal.Unknown0x04 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x04 != 0x0028)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x04 has unexpected value of "
                    + Internal.Unknown0x04);
            }

            Internal.Unknown0x06 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x06 != 0x0240 && Internal.Unknown0x06 != 0x0242 && Internal.Unknown0x06 != 0x0243)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x06 has unexpected value of "
                    + Internal.Unknown0x06);
            }

            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x08 != 0x49749732 && Internal.Unknown0x08 != 0x49749736)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x08 has unexpected value of "
                    + Internal.Unknown0x08);
            }

            Internal.Unknown0x0C = context.Reader.ReadUInt32();
            if (Internal.Unknown0x0C > 0x0E)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x0C has unexpected value of "
                    + Internal.Unknown0x0C);
            }

            Internal.Unknown0x10 = context.Reader.ReadSingle();
            if (Internal.Unknown0x10 >= 1f)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x10 has unexpected value of "
                    + Internal.Unknown0x10);
            }

            Internal.Unknown0x14 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x14 != 0x00CC)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x14 has unexpected value of "
                    + Internal.Unknown0x14);
            }

            Internal.Unknown0x16 = context.Reader.ReadUInt16();
            if (Internal.Unknown0x16 != 0x0000 && Internal.Unknown0x16 != 0x0100)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x16 has unexpected value of "
                    + Internal.Unknown0x16);
            }

            Internal.Unknown0x18 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x18 != 0)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x18 has unexpected value of "
                    + Internal.Unknown0x18);
            }

            Internal.Unknown0x1C = context.Reader.ReadUInt16();
            if (Internal.Unknown0x1C != 0x2001)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x1C has unexpected value of "
                    + Internal.Unknown0x1C);
            }

            Internal.Unknown0x1E = context.Reader.ReadByte();
            if (Internal.Unknown0x1E != 1 && Internal.Unknown0x1E != 0x05)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x1E has unexpected value of "
                    + Internal.Unknown0x1E);
            }

            Internal.Unknown0x1F = context.Reader.ReadByte();
            if (Internal.Unknown0x1F != 1 && Internal.Unknown0x1F != 4)
            {
                context.Warnings.Add("FmatMaterialStructure.Unknown0x1F has unexpected value of "
                    + Internal.Unknown0x1F);
            }

            // 16 empty bytes
            if (!Array.TrueForAll(context.Reader.ReadUInt32s(4), (i) => { return i == 0; }))
            {
                context.Warnings.Add("FmatMaterialStructure padding not empty");
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatMaterialStructure"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------
            
            /// <summary>
            /// Gets or sets an unknown variable at offset 0x00. The value always seems to be less than 0x00000014.
            /// </summary>
            public uint Unknown0x00
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0x0028.
            /// </summary>
            public ushort Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be 0x0240, 0x0242 or 0x0243.
            /// </summary>
            public ushort Unknown0x06
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 0x49749732 or 0x49749736.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0C. The value always seems to be less than 0x0000000E.
            /// </summary>
            public uint Unknown0x0C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x10. The value always seems to be 1.0f.
            /// </summary>
            public float Unknown0x10
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x14. The value always seems to be 0x00CC.
            /// </summary>
            public ushort Unknown0x14
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x16. The value always seems to be 0x0000 or 0x0100.
            /// </summary>
            public ushort Unknown0x16
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x18. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x18
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x1C. The value always seems to be 0x2001.
            /// </summary>
            public ushort Unknown0x1C
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x1E. The value always seems to be 1 or 5.
            /// </summary>
            public byte Unknown0x1E
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x1F. The value always seems to be 1 or 4.
            /// </summary>
            public byte Unknown0x1F
            {
                get;
                set;
            }
        }
    }
}
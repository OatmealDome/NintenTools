namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System;
    using System.Diagnostics;
    using Syroot.NintenTools.IO;
    using Syroot.NintenTools.Maths;

    /// <summary>
    /// Represents a material parameter in an FMAT subsection, describing how a value is passed to a uniform variable in
    /// shader code.
    /// </summary>
    [DebuggerDisplay("Material Parameter {Type} {Name} = {Value}")]
    public class FmatMaterialParameter
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatMaterialParameter"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// The provided <see cref="BinaryDataReader"/> reads over the material parameter data section in the correct
        /// endianness.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        /// <param name="dataReader">The <see cref="BinaryDataReader"/> to read material parameter data with.</param>
        internal FmatMaterialParameter(BfresLoaderContext context, BinaryDataReader dataReader)
        {
            Load(context, dataReader);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatMaterialParameter"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the variable. To get a correctly typed variable, use one of the Get methods.
        /// </summary>
        public FmatMaterialParameterType Type
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

        /// <summary>
        /// Gets or sets the value of this attribute, which type is defined by <see cref="Type"/>.
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context, BinaryDataReader dataReader)
        {
            Internal = new Internals();

            Type = (FmatMaterialParameterType)context.Reader.ReadByte();
            if (!Enum.IsDefined(typeof(FmatMaterialParameterType), Type))
            {
                throw new BfresException("FmatMaterialParameter.Type invalid");
            }

            Internal.Size = context.Reader.ReadByte();
            Internal.Offset = context.Reader.ReadUInt16();

            Internal.Unknown0x04 = context.Reader.ReadUInt32();
            Internal.Unknown0x08 = context.Reader.ReadUInt32();

            Internal.Index = context.Reader.ReadUInt16();
            Internal.IndexRepeated = context.Reader.ReadUInt16();
            if (Internal.Index != Internal.IndexRepeated)
            {
                context.Warnings.Add("FmatMaterial.Index has not the same value as FmatMaterial.IndexRepeated");
            }

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;

            // Load the value from the given BinaryDataReader.
            dataReader.Position = Internal.Offset;
            switch (Type)
            {
                case FmatMaterialParameterType.Int32:
                    Value = dataReader.ReadInt32();
                    break;
                case FmatMaterialParameterType.Single:
                    Value = dataReader.ReadSingle();
                    break;
                case FmatMaterialParameterType.Vector2F:
                    Value = dataReader.ReadVector2F();
                    break;
                case FmatMaterialParameterType.Vector3F:
                    Value = dataReader.ReadVector3F();
                    break;
                case FmatMaterialParameterType.Vector4F:
                    Value = dataReader.ReadVector4F();
                    break;
                case FmatMaterialParameterType.Matrix2x3:
                    // Stored in column-major order, our structure however is row-major.
                    float m11 = dataReader.ReadSingle();
                    float m21 = dataReader.ReadSingle();
                    float m12 = dataReader.ReadSingle();
                    float m22 = dataReader.ReadSingle();
                    float m13 = dataReader.ReadSingle();
                    float m23 = dataReader.ReadSingle();
                    Value = new Matrix2x3(m11, m12, m13, m21, m22, m23);
                    break;
            }
        }
        
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatMaterialParameter"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the length of the parameter data in the data section, in bytes.
            /// </summary>
            public byte Size
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the parameter data, relative to the start of the data section, in bytes.
            /// </summary>
            public ushort Offset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0xFFFFFFFF.
            /// </summary>
            public uint Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 0.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index in the parent's material parameter array.
            /// </summary>
            public ushort Index
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index in the parent's material parameter array.
            /// </summary>
            public ushort IndexRepeated
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the parameter name in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }
        }
    }

    /// <summary>
    /// Represents the possible types an <see cref="FmatMaterialParameter"/> can reference.
    /// </summary>
    public enum FmatMaterialParameterType : byte
    {
        /// <summary>
        /// Represents a signed integer, having a size of 4 bytes.
        /// </summary>
        Int32 = 0x04,

        /// <summary>
        /// Represents a float, having a size of 4 bytes.
        /// </summary>
        Single = 0x0C,

        /// <summary>
        /// Represents a float vector with 2 components, having a size of 8 bytes.
        /// </summary>
        Vector2F = 0x0D,

        /// <summary>
        /// Represents a float vector with 3 components, having a size of 12 bytes.
        /// </summary>
        Vector3F = 0x0E,

        /// <summary>
        /// Represents a float vector with 4 components, having a size of 16 bytes.
        /// </summary>
        Vector4F = 0x0F,

        /// <summary>
        /// Represents a 2x3 texture matrix, having a size of 24 bytes.
        /// </summary>
        Matrix2x3 = 0x1E
    }
}
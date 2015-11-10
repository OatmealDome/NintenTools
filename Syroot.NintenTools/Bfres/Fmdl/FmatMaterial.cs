namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Syroot.NintenTools.Bfres.Ftex;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents an FMAT subsection in a FMDL section which describes a material to draw polygons with.
    /// </summary>
    [DebuggerDisplay("Material {Name}")]
    public class FmatMaterial
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatMaterial"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatMaterial(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatMaterial"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name to describe the resulting material.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FmatRenderParameter"/> instances which describe the values of uniform
        /// shader variables providing render information.
        /// </summary>
        public List<FmatRenderParameter> RenderParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FmatTextureSelector"/> instances referencing a texture by name and
        /// offset.
        /// </summary>
        public List<FmatTextureSelector> TextureSelectors
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the list of <see cref="FmatTextureAttributeSelector"/> instances which describe the values of attribute shader
        /// variables.
        /// </summary>
        public List<FmatTextureAttributeSelector> TextureAttributeSelectors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FmatShadowParameter"/> instances which describe the values of uniform
        /// shader variables.
        /// </summary>
        public List<FmatMaterialParameter> MaterialParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unknown <see cref="FmatMaterialStructure"/> structure.
        /// </summary>
        public FmatMaterialStructure MaterialStructure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="FmatShaderControl"/> structure controlling how shaders are used by the model.
        /// </summary>
        public FmatShaderControl ShaderControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FmatShadowParameter"/> instances which describe the values of uniform
        /// shader variables for shadowing.
        /// </summary>
        public List<FmatShadowParameter> ShadowParameters
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FMAT")
            {
                throw new BfresException("FmatMaterial identifier invalid");
            }

            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;

            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x08 != 1)
            {
                context.Warnings.Add("FmatMaterial.Unknown0x08 has unexpected value of " + Internal.Unknown0x08);
            }

            Internal.Index = context.Reader.ReadUInt16();

            Internal.RenderParameterCount = context.Reader.ReadUInt16();
            Internal.TextureSelectorCount = context.Reader.ReadByte();
            Internal.TextureAttributeSelectorCount = context.Reader.ReadByte();
            Internal.MaterialParameterCount = context.Reader.ReadUInt16();
            Internal.MaterialParameterDataSize = context.Reader.ReadUInt32();

            Internal.Unknown0x18 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x18 != 0 && Internal.Unknown0x18 != 1 && Internal.Unknown0x18 != 2)
            {
                context.Warnings.Add("FmatMaterial.Unknown0x18 has unexpected value of " + Internal.Unknown0x18);
            }

            Internal.RenderParameterIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.MaterialStructureOffset = context.Reader.ReadBfresOffset();
            Internal.ShaderControlOffset = context.Reader.ReadBfresOffset();
            Internal.TextureSelectorOffset = context.Reader.ReadBfresOffset();
            Internal.TextureAttributeSelectorOffset = context.Reader.ReadBfresOffset();
            Internal.TextureAttributeSelectorIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.MaterialParameterOffset = context.Reader.ReadBfresOffset();
            Internal.MaterialParameterIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.MaterialParameterDataOffset = context.Reader.ReadBfresOffset();
            Internal.ShadowParameterIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.Unknown0x44 = context.Reader.ReadBfresOffset();

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadRenderParameters(context);
                LoadTextureSelectors(context);
                LoadTextureAttributeSelectors(context);
                LoadMaterialParameters(context);
                LoadMaterialStructure(context);
                LoadShaderControl(context);
                LoadShadowParameters(context);
            }
        }

        private void LoadRenderParameters(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.RenderParameterIndexGroupOffset.ToFile;
            Internal.RenderParameterIndexGroup = new BfresIndexGroup(context);
            if (Internal.RenderParameterIndexGroup.NodeCount != Internal.RenderParameterCount)
            {
                context.Warnings.Add("FmatMaterial.RenderParameterIndexGroup has node count unequal to header");
            }

            // Load the referenced instances into the list.
            RenderParameters = new List<FmatRenderParameter>((int)Internal.RenderParameterIndexGroup.NodeCount);
            for (int i = 1; i < Internal.RenderParameterIndexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = Internal.RenderParameterIndexGroup[i];
                context.Reader.Position = node.DataPointer.ToFile;
                RenderParameters.Add(new FmatRenderParameter(context));
            }
        }

        private void LoadTextureSelectors(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.TextureSelectorOffset.ToFile;
            TextureSelectors = new List<FmatTextureSelector>(Internal.TextureSelectorCount);
            for (int i = 0; i < Internal.TextureSelectorCount; i++)
            {
                TextureSelectors.Add(new FmatTextureSelector(context));
            }
        }

        private void LoadTextureAttributeSelectors(BfresLoaderContext context)
        {
            // Load the array.
            context.Reader.Position = Internal.TextureAttributeSelectorOffset.ToFile;
            TextureAttributeSelectors = new List<FmatTextureAttributeSelector>(Internal.TextureAttributeSelectorCount);
            for (int i = 0; i < Internal.TextureAttributeSelectorCount; i++)
            {
                TextureAttributeSelectors.Add(new FmatTextureAttributeSelector(context));
            }

            // Load the index group, just for internal use and fun.
            context.Reader.Position = Internal.TextureAttributeSelectorIndexGroupOffset.ToFile;
            Internal.TextureAttributeSelectorIndexGroup = new BfresIndexGroup(context);
            if (Internal.TextureAttributeSelectorIndexGroup.NodeCount != Internal.TextureAttributeSelectorCount)
            {
                context.Warnings.Add("FmatMaterial.TextureAttributeSelectorIndexGroup has node count unequal to "
                    + "header");
            }
        }
        
        private void LoadMaterialParameters(BfresLoaderContext context)
        {
            // Load the data before the parameters to extract their values.
            context.Reader.Position = Internal.MaterialParameterDataOffset.ToFile;
            Internal.MaterialParameterData = context.Reader.ReadBytes((int)Internal.MaterialParameterDataSize);

            // Load the array, and pass a reader on the parameter data array.
            context.Reader.Position = Internal.MaterialParameterOffset.ToFile;
            MaterialParameters = new List<FmatMaterialParameter>(Internal.MaterialParameterCount);
            using (BinaryDataReader parameterDataReader = new BinaryDataReader(
                new MemoryStream(Internal.MaterialParameterData)))
            {
                parameterDataReader.ByteOrder = ByteOrder.BigEndian;
                for (int i = 0; i < Internal.MaterialParameterCount; i++)
                {
                    MaterialParameters.Add(new FmatMaterialParameter(context, parameterDataReader));
                }
            }

            // Load the index group, just for internal use and because I feel nasty today.
            context.Reader.Position = Internal.MaterialParameterIndexGroupOffset.ToFile;
            Internal.MaterialParameterIndexGroup = new BfresIndexGroup(context);
            if (Internal.MaterialParameterIndexGroup.NodeCount != Internal.MaterialParameterCount)
            {
                context.Warnings.Add("FmatMaterial.MaterialParameterIndexGroup has node count unequal to header");
            }
        }
        
        private void LoadMaterialStructure(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.MaterialStructureOffset.ToFile;
            MaterialStructure = new FmatMaterialStructure(context);
        }

        private void LoadShaderControl(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.ShaderControlOffset.ToFile;
            ShaderControl = new FmatShaderControl(context);
        }

        private void LoadShadowParameters(BfresLoaderContext context)
        {
            // Load the index group.
            if (!Internal.ShadowParameterIndexGroupOffset.IsEmpty)
            {
                context.Reader.Position = Internal.ShadowParameterIndexGroupOffset.ToFile;
                Internal.ShadowParameterIndexGroup = new BfresIndexGroup(context);
            }

            // Load the referenced shadow parameters into the list.
            ShadowParameters = new List<FmatShadowParameter>();
            if (Internal.ShadowParameterIndexGroup != null)
            {
                for (int i = 1; i < Internal.ShadowParameterIndexGroup.Nodes.Length; i++)
                {
                    BfresIndexGroupNode node = Internal.ShadowParameterIndexGroup[i];
                    context.Reader.Position = node.DataPointer.ToFile;
                    ShadowParameters.Add(new FmatShadowParameter(context));
                }
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatMaterial"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the file name without the extension in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 1.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of this material in the parent's material array.
            /// </summary>
            public ushort Index
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmatRenderParameter"/> instances in the
            /// <see cref="RenderParameters"/> list.
            /// </summary>
            public ushort RenderParameterCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmatTextureSelector"/> instances in the
            /// <see cref="TextureSelectors"/> list.
            /// </summary>
            public byte TextureSelectorCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmatTextureAttributeSelector"/> instances in the
            /// <see cref="TextureAttributeSelector"/> list.
            /// </summary>
            public byte TextureAttributeSelectorCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FmatMaterialParameter"/> instances in the
            /// <see cref="MaterialParameters"/> list.
            /// </summary>
            public ushort MaterialParameterCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the size in bytes of the structure holding the data for the
            /// <see cref="FmatMaterialParameter"/> instances.
            /// </summary>
            public uint MaterialParameterDataSize
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x18. The value always seems to be 0, 1 or 2.
            /// </summary>
            public uint Unknown0x18
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FmatRenderParameter"/> instance.
            /// </summary>
            public BfresOffset RenderParameterIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the unknown <see cref="FmatMaterialStructure"/> instance.
            /// </summary>
            public BfresOffset MaterialStructureOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FmatRenderParameter"/> instance.
            /// </summary>
            public BfresOffset ShaderControlOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FmatTextureSelector"/> instance.
            /// </summary>
            public BfresOffset TextureSelectorOffset
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the offset to the first <see cref="FmatTextureAttributeSelector"/> instance.
            /// </summary>
            public BfresOffset TextureAttributeSelectorOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the index group referencing <see cref="FmatTextureAttributeSelector"/>
            /// instance by name.
            /// </summary>
            public BfresOffset TextureAttributeSelectorIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the first <see cref="FmatMaterialParameter"/> instance.
            /// </summary>
            public BfresOffset MaterialParameterOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the index group referencing <see cref="FmatMaterialParameter"/> instances by
            /// name.
            /// </summary>
            public BfresOffset MaterialParameterIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the structure holding the <see cref="FmatMaterialParameter"/> data.
            /// </summary>
            public BfresOffset MaterialParameterDataOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the index group referencing <see cref="FmatShadowParameter"/> instances by
            /// name.
            /// </summary>
            public BfresOffset ShadowParameterIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to unknown data, pointing to 12 empty bytes, which may not always be set.
            /// </summary>
            public BfresOffset Unknown0x44
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing <see cref="FmatRenderParameter"/> by name.
            /// </summary>
            public BfresIndexGroup RenderParameterIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing <see cref="FmatTextureAttributeSelector"/> by
            /// name.
            /// </summary>
            public BfresIndexGroup TextureAttributeSelectorIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the raw material data byte array for the data referenced in
            /// <see cref="FmatMaterialParameter"/> as their value.
            /// </summary>
            public byte[] MaterialParameterData
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing <see cref="FmatMaterialParameter"/> by name.
            /// </summary>
            public BfresIndexGroup MaterialParameterIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets <see cref="BfresIndexGroup"/> referencing <see cref="FmatShadowParameter"/> by name.
            /// </summary>
            public BfresIndexGroup ShadowParameterIndexGroup
            {
                get;
                set;
            }
        }
    }
}
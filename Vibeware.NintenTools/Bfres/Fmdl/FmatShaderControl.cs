namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System;
    using System.Collections.Generic;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents the shader control structure in an FMAT subsection, describing how a shader is used by the material.
    /// </summary>
    public class FmatShaderControl
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatShaderControl"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatShaderControl(BfresLoaderContext context)
        {
            Load(context);
        }
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="XXXX"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the vertex shader.
        /// </summary>
        public string Shader1Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the pixel shader.
        /// </summary>
        public string Shader2Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of vertex shader input variable names.
        /// </summary>
        public List<string> VertexShaderInputs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of pixel shader input variable names.
        /// </summary>
        public List<string> PixelShaderInputs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of shader input uniform variable names.
        /// </summary>
        public List<string> Parameters
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Shader1NameOffset = context.Reader.ReadBfresNameOffset();
            Shader1Name = Internal.Shader1NameOffset.Name;
            Internal.Shader2NameOffset = context.Reader.ReadBfresNameOffset();
            Shader2Name = Internal.Shader2NameOffset.Name;

            Internal.Unknown0x08 = context.Reader.ReadUInt32();
            if (Internal.Unknown0x08 != 0 && Internal.Unknown0x08 != 1)
            {
                context.Warnings.Add("FmatShaderControl.Unknown0x08 has unexpected value of " + Internal.Unknown0x08);
            }

            Internal.VertexShaderInputCount = context.Reader.ReadByte();
            Internal.PixelShaderInputCount = context.Reader.ReadByte();
            Internal.ParameterCount = context.Reader.ReadUInt16();
            Internal.VertexShaderInputIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.PixelShaderInputIndexGroupOffset = context.Reader.ReadBfresOffset();
            Internal.ParameterIndexGroupOffset = context.Reader.ReadBfresOffset();

            // Restore the position after the header, to allow consecutive header reads for the parent.
            using (context.Reader.TemporarySeek())
            {
                LoadIndexGroups(context);
            }
        }

        private void LoadIndexGroups(BfresLoaderContext context)
        {
            context.Reader.Position = Internal.VertexShaderInputIndexGroupOffset.ToFile;
            Internal.VertexShaderInputIndexGroup = new BfresIndexGroup(context);
            // Put the nodes into the string list.
            VertexShaderInputs = new List<string>();
            for (int i = 1; i < Internal.VertexShaderInputIndexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = Internal.VertexShaderInputIndexGroup.Nodes[i];
                VertexShaderInputs.Add(node.Name);
            }
            
            context.Reader.Position = Internal.PixelShaderInputIndexGroupOffset.ToFile;
            Internal.PixelShaderInputIndexGroup = new BfresIndexGroup(context);
            // Put the nodes into the string list.
            PixelShaderInputs = new List<string>();
            for (int i = 1; i < Internal.PixelShaderInputIndexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = Internal.PixelShaderInputIndexGroup.Nodes[i];
                PixelShaderInputs.Add(node.Name);
            }

            context.Reader.Position = Internal.ParameterIndexGroupOffset.ToFile;
            Internal.ParameterIndexGroup = new BfresIndexGroup(context);
            // Put the nodes into the string list.
            Parameters = new List<string>();
            for (int i = 1; i < Internal.ParameterIndexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = Internal.ParameterIndexGroup.Nodes[i];
                Parameters.Add(node.Name);
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatShaderControl"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to <see cref="Shader1Name"/> in the string table.
            /// </summary>
            public BfresNameOffset Shader1NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to <see cref="Shader2Name"/> in the string table.
            /// </summary>
            public BfresNameOffset Shader2NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x08. The value always seems to be 0x00000000 or 0x00000001.
            /// </summary>
            public uint Unknown0x08
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="String"/> instances in the <see cref="VertexShaderInputs"/> list.
            /// </summary>
            public byte VertexShaderInputCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="String"/> instances in the <see cref="PixelShaderInputs"/> list.
            /// </summary>
            public byte PixelShaderInputCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="String"/> instances in the <see cref="Parameters"/> list.
            /// </summary>
            public ushort ParameterCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing the items in the
            /// <see cref="VertexShaderInputs"/> list by name.
            /// </summary>
            public BfresOffset VertexShaderInputIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing the items in the
            /// <see cref="PixelShaderInputs"/> list by name.
            /// </summary>
            public BfresOffset PixelShaderInputIndexGroupOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the <see cref="BfresIndexGroup"/> referencing the items in the
            /// <see cref="Parameters"/> list by name.
            /// </summary>
            public BfresOffset ParameterIndexGroupOffset
            {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing the items in the
            /// <see cref="VertexShaderInputs"/> list by name.
            /// </summary>
            public BfresIndexGroup VertexShaderInputIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> referencing the items in the
            /// <see cref="PixelShaderInputs"/> list by name.
            /// </summary>
            public BfresIndexGroup PixelShaderInputIndexGroup
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets to the <see cref="BfresIndexGroup"/> referencing the items in the <see cref="Parameters"/>
            /// list by name.
            /// </summary>
            public BfresIndexGroup ParameterIndexGroup
            {
                get;
                set;
            }
        }
    }
}
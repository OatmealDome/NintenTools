namespace Vibeware.NintenTools.Bfres.Fmdl
{
    using System.Diagnostics;
    using Vibeware.NintenTools.Bfres.Ftex;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents a texture selector in an FMAT subsection, describing the name and offset of the texture to reference.
    /// </summary>
    [DebuggerDisplay("Texture Selector {Name}")]
    public class FmatTextureSelector
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="FmatTextureSelector"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FmatTextureSelector(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FmatTextureSelector"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the referenced texture.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the referenced <see cref="FtexTexture"/> instance.
        /// </summary>
        public FtexSection Texture
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
            Internal.TextureOffset = context.Reader.ReadBfresOffset();

            // Referenced texture is linked after loading and mapping the raw data.
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FmatRenderParameter"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the offset to the texture selector's <see cref="Name"/> in the string table.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the FTEX structure in the BFRES file.
            /// </summary>
            public BfresOffset TextureOffset
            {
                get;
                set;
            }
        }
    }
}
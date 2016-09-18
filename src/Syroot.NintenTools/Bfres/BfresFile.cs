namespace Syroot.NintenTools.Bfres
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Syroot.IO;
    using Syroot.NintenTools.Bfres.Embedded;
    using Syroot.NintenTools.Bfres.Fmdl;
    using Syroot.NintenTools.Bfres.Fscn;
    using Syroot.NintenTools.Bfres.Fsha;
    using Syroot.NintenTools.Bfres.Fshu;
    using Syroot.NintenTools.Bfres.Fska;
    using Syroot.NintenTools.Bfres.Ftex;
    using Syroot.NintenTools.Bfres.Ftxp;
    using Syroot.NintenTools.Bfres.Fvis;
    using Syroot.NintenTools.IO;

    /// <summary>
    /// Represents a BFRES file and its contents.
    /// </summary>
    [Obsolete("The BFRES API requires a big update. Use on your own fault.")]
    public class BfresFile
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresFile"/> class.
        /// </summary>
        public BfresFile()
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="BfresFile"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the internally stored name of this BFRES file.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of FMDL sections describing model data.
        /// </summary>
        public List<FmdlSection> FmdlSections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of FTEX sections describing texture data.
        /// </summary>
        public List<FtexSection> FtexSections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of FSKA sections.
        /// </summary>
        public List<FskaSection> FskaSections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of FSHU sections.
        /// </summary>
        public List<FshuSection> FshuSections
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of FTXP sections.
        /// </summary>
        public List<FtxpSection> FtxpSections
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of FVIS sections describing bone visibility.
        /// </summary>
        public List<FvisSection> FvisBoneVisibilitySections
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of FSHA sections.
        /// </summary>
        public List<FshaSection> FshaSections
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of FSCN sections describing scene data.
        /// </summary>
        public List<FscnSection> FscnScenes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of embedded files.
        /// </summary>
        public List<EmbeddedFile> EmbeddedFiles
        {
            get;
            private set;
        }
        
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the data from the file with the given name, and returns a collection of warnings which might have been
        /// raised when loading.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        /// <returns>The list of warnings raised when loading the data.</returns>
        public List<string> Load(string fileName)
        {
            return Load(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        /// <summary>
        /// Loads the data from the given stream, and returns a collection of warnings which might have been raised when
        /// loading.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <returns>The list of warnings raised when loading the data.</returns>
        public List<string> Load(Stream stream)
        {
            BfresLoaderContext context;

            using (BinaryDataReader reader = new BinaryDataReader(stream))
            {
                context = new BfresLoaderContext(this, reader);

                // A BFRES file is always stored in big-endian, embedded file endianness is determined by header.
                reader.ByteOrder = ByteOrder.BigEndian;

                Load(context);
            }

            return context.Warnings;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            // Read magic bytes.
            if (context.Reader.ReadString(4) != "FRES")
            {
                throw new BfresException("BfresFile identifier invalid");
            }
            
            // The following 4 bytes could also be an uint. BFRES itself is always big-endian, the statement in the wiki
            // might be wrong, as the BOM should only affect embedded files.

            Internal.Unknown0x04 = context.Reader.ReadByte();
            if (Internal.Unknown0x04 != 0x03)
            {
                context.Warnings.Add("BfresFile.Unknown0x04 has unexpected value of " + Internal.Unknown0x04);
            }

            Internal.Unknown0x05 = context.Reader.ReadByte();
            if (Internal.Unknown0x05 != 0x00 && Internal.Unknown0x05 != 0x03 && Internal.Unknown0x05 != 0x04)
            {
                context.Warnings.Add("BfresFile.Unknown0x05 has unexpected value of " + Internal.Unknown0x05);
            }

            Internal.Unknown0x06 = context.Reader.ReadByte();
            if (Internal.Unknown0x06 != 0x00)
            {
                context.Warnings.Add("BfresFile.Unknown0x06 has unexpected value of " + Internal.Unknown0x06);
            }

            Internal.Unknown0x07 = context.Reader.ReadByte();
            if (Internal.Unknown0x07 != 0x01 && Internal.Unknown0x07 != 0x02 && Internal.Unknown0x07 != 0x04)
            {
                context.Warnings.Add("BfresFile.Unknown0x07 has unexpected value of " + Internal.Unknown0x07);
            }

            Internal.ByteOrder = (ByteOrder)context.Reader.ReadUInt16();
            if (!Enum.IsDefined(typeof(ByteOrder), Internal.ByteOrder))
            {
                throw new BfresException("BfresFile.ByteOrder invalid");
            }

            Internal.Unknown0x0A = context.Reader.ReadUInt16();
            if (Internal.Unknown0x0A != 0x0010)
            {
                context.Warnings.Add("BfresFile.Unknown0x0A has unexpected value of " + Internal.Unknown0x0A);
            }

            // Read file properties.
            Internal.FileLength = context.Reader.ReadUInt32();
            Internal.FileAlignment = context.Reader.ReadUInt32();
            Internal.FileNameOffset = context.Reader.ReadBfresNameOffset();
            FileName = Internal.FileNameOffset.Name;

            // Read string table properties.
            Internal.StringTableLength = context.Reader.ReadUInt32();
            Internal.StringTableOffset = context.Reader.ReadBfresOffset();

            // Read subsection index group properties.
            Internal.SubsectionIndexGroupOffsets = context.Reader.ReadBfresOffsets(12);
            Internal.SubsectionIndexGroupCounts = context.Reader.ReadUInt16s(12);

            Internal.SubsectionIndexGroups = new List<BfresIndexGroup>(12);
            for (int i = 0; i < 12; i++)
            {
                BfresOffset indexGroupOffset = Internal.SubsectionIndexGroupOffsets[i];
                if (indexGroupOffset.IsEmpty)
                {
                    Internal.SubsectionIndexGroups.Add(null);
                }
                else
                {
                    // Load the index group.
                    context.Reader.Position = indexGroupOffset.ToFile;
                    BfresIndexGroup indexGroup = new BfresIndexGroup(context);

                    // Check if the node count specified by the group itself matches the one specified in the header.
                    if (Internal.SubsectionIndexGroupCounts[i] != indexGroup.Nodes.Length - 1)
                    {
                        context.Warnings.Add(string.Format("BfresFileSubsectionIndexGroupCounts[{0}] does not match "
                            + "count of index group.", Internal.SubsectionIndexGroupCounts[i]));
                    }

                    Internal.SubsectionIndexGroups.Add(indexGroup);
                }
            }
            
            LoadSubsections(context);

            // Link section instances to properties of other instances, to make an object-oriented design possible.
            SatisfyReferences(context);
        }

        private void LoadSubsections(BfresLoaderContext context)
        {
            // Go through the index groups, and load the included subsections appropriately.
            for (int i = 0; i < Internal.SubsectionIndexGroups.Count; i++)
            {
                BfresIndexGroup indexGroup = Internal.SubsectionIndexGroups[i];
                if (indexGroup != null)
                {
                    switch ((BfresSubsectionType)i)
                    {
                        case BfresSubsectionType.Fmdl0:
                            LoadFmdlSubsections(context);
                            break;
                        case BfresSubsectionType.Ftex1:
                            LoadFtexSubsections(context);
                            break;
                        case BfresSubsectionType.Fska2:
                            LoadFskaSubsections(context);
                            break;
                        case BfresSubsectionType.Fshu3:
                        case BfresSubsectionType.Fshu4:
                        case BfresSubsectionType.Fshu5:
                            LoadFshuSubsections(context, i);
                            break;
                        case BfresSubsectionType.Ftxp6:
                            LoadFtxpSubsections(context);
                            break;
                        case BfresSubsectionType.Fvis7:
                            context.Warnings.Add("BfresFile.SubsectionIndexGroups[7] normally unused in Mario Kart 8.");
                            LoadFvisSubsections(context);
                            break;
                        case BfresSubsectionType.Fvis8:
                            LoadFvisSubsections(context);
                            break;
                        case BfresSubsectionType.Fsha9:
                            LoadFshaSubsections(context);
                            break;
                        case BfresSubsectionType.Fscn10:
                            LoadFscnSubsections(context);
                            break;
                        case BfresSubsectionType.EmbeddedFile11:
                            LoadEmbeddedFiles(context);
                            break;
                    }
                }
            }
        }

        private void LoadFmdlSubsections(BfresLoaderContext context)
        {
            BfresIndexGroup indexGroup = Internal.SubsectionIndexGroups[(int)BfresSubsectionType.Fmdl0];
            FmdlSections = new List<FmdlSection>(indexGroup.Nodes.Length - 1);
            for (int i = 1; i < indexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = indexGroup.Nodes[i];
                // Position the reader and load the section into the list.
                context.Reader.Position = node.DataPointer.ToFile;
                FmdlSections.Add(new FmdlSection(context));
            }
        }

        private void LoadFtexSubsections(BfresLoaderContext context)
        {
            BfresIndexGroup indexGroup = Internal.SubsectionIndexGroups[(int)BfresSubsectionType.Ftex1];
            FtexSections = new List<FtexSection>(indexGroup.Nodes.Length - 1);
            for (int i = 1; i < indexGroup.Nodes.Length; i++)
            {
                BfresIndexGroupNode node = indexGroup.Nodes[i];
                // Position the reader and load the section into the list.
                context.Reader.Position = node.DataPointer.ToFile;
                FtexSections.Add(new FtexSection(context));
            }
        }

        private void LoadFskaSubsections(BfresLoaderContext context)
        {
        }

        private void LoadFshuSubsections(BfresLoaderContext context, int indexGroupIndex)
        {
        }

        private void LoadFtxpSubsections(BfresLoaderContext context)
        {
        }

        private void LoadFvisSubsections(BfresLoaderContext context)
        {
        }

        private void LoadFshaSubsections(BfresLoaderContext context)
        {
        }

        private void LoadFscnSubsections(BfresLoaderContext context)
        {
        }

        private void LoadEmbeddedFiles(BfresLoaderContext context)
        {
        }

        private void SatisfyReferences(BfresLoaderContext context)
        {
            SatisfyFshpModels(context);
            SatisfyTextureSelectors(context);
        }

        private void SatisfyFshpModels(BfresLoaderContext context)
        {
            // Go through each FMDL section in the file.
            foreach (FmdlSection fmdlSection in FmdlSections)
            {
                // Go through each model in that FMDL section.
                foreach (FshpModel fshpModel in fmdlSection.Models)
                {
                    // Link the FsklBone.
                    fshpModel.Bone = fmdlSection.Skeleton.Bones[fshpModel.Internal.BoneIndex];

                    // Link the FmatMaterial.
                    fshpModel.Material = fmdlSection.Materials[fshpModel.Internal.MaterialIndex];

                    // Link the FvtxVertexData.
                    fshpModel.VertexBuffer = fmdlSection.VertexBuffers[fshpModel.Internal.VertexBufferIndex];
                }
            }
        }
        
        private void SatisfyTextureSelectors(BfresLoaderContext context)
        {
            // Link FtexSection instances to the FmatMaterial texture selectors.
            BfresIndexGroup ftexIndexGroup = Internal.SubsectionIndexGroups[(int)BfresSubsectionType.Ftex1];

            // Go through each FMDL section in the file.
            foreach (FmdlSection fmdlSection in FmdlSections)
            {
                // Go through each material of that FMDL section.
                foreach (FmatMaterial fmatMaterial in fmdlSection.Materials)
                {
                    // Go through each texture of that material.
                    foreach (FmatTextureSelector textureSelector in fmatMaterial.TextureSelectors)
                    {
                        int? textureIndex = ftexIndexGroup.GetIndexByName(textureSelector.Name);
                        if (textureIndex == null)
                        {
                            context.Warnings.Add("FmatTextureSelector references FtexSection with unknown name.");
                        }
                        else
                        {
                            // Link the referenced texture.
                            textureSelector.Texture = FtexSections[textureIndex.Value - 1];
                        }
                    }
                }
            }
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="BfresFile"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be 0x03.
            /// </summary>
            public byte Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x05. The value always seems to be 0x00, 0x03 or 0x04.
            /// </summary>
            public byte Unknown0x05
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x06. The value always seems to be 0x00.
            /// </summary>
            public byte Unknown0x06
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x07. The value always seems to be 0x01, 0x02 or 0x04.
            /// </summary>
            public byte Unknown0x07
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the byte order of embedded files. This does not affect the remainder of the BFRES file.
            /// </summary>
            public ByteOrder ByteOrder
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x0A. The value always seems to be 0x0010.
            /// </summary>
            public ushort Unknown0x0A
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the file length including all headers, in bytes.
            /// </summary>
            public uint FileLength
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the alignment of data in the file, in bytes, and should be a power of 2.
            /// </summary>
            public uint FileAlignment
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the file name without the extension in the string table.
            /// </summary>
            public BfresNameOffset FileNameOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the length of the string table, in bytes.
            /// </summary>
            public uint StringTableLength
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset of the string table.
            /// </summary>
            public BfresOffset StringTableOffset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offsets to the 12 BFRES file index groups.
            /// </summary>
            public BfresOffset[] SubsectionIndexGroupOffsets
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of subsection in the BFRES file index groups.
            /// </summary>
            public ushort[] SubsectionIndexGroupCounts
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the <see cref="BfresIndexGroup"/> instances for the subsections.
            /// </summary>
            public List<BfresIndexGroup> SubsectionIndexGroups
            {
                get;
                set;
            }
        }
    }

    /// <summary>
    /// Represents the indices to typical subsection file types in a BFRES file.
    /// </summary>
    public enum BfresSubsectionType
    {
        /// <summary>
        /// An FMDL subsection at index 0.
        /// </summary>
        Fmdl0 = 0,

        /// <summary>
        /// An FTEX subsection at index 1.
        /// </summary>
        Ftex1 = 1,

        /// <summary>
        /// An FSKA subsection at index 2.
        /// </summary>
        Fska2 = 2,

        /// <summary>
        /// An FSHU subsection at index 3.
        /// </summary>
        Fshu3 = 3,

        /// <summary>
        /// An FSHU subsection at index 4.
        /// </summary>
        Fshu4 = 4,

        /// <summary>
        /// An FSHU subsection at index 5.
        /// </summary>
        Fshu5 = 5,

        /// <summary>
        /// An FTXP subsection at index 6.
        /// </summary>
        Ftxp6 = 6,

        /// <summary>
        /// An FVIS subsection at index 7, normally unused in Mario Kart 8, but seen in Super Mario Maker.
        /// </summary>
        Fvis7 = 7,

        /// <summary>
        /// An FVIS subsection at index 8.
        /// </summary>
        Fvis8 = 8,

        /// <summary>
        /// An FSHA subsection at index 9.
        /// </summary>
        Fsha9 = 9,

        /// <summary>
        /// An FSCN subsection at index 10.
        /// </summary>
        Fscn10 = 10,

        /// <summary>
        /// An embedded file at index 11.
        /// </summary>
        EmbeddedFile11 = 11
    }
}

namespace Vibeware.NintenTools.BfresConverter.Converters.ObjMtl
{
    using System.Globalization;
    using System.IO;
    using Vibeware.NintenTools.Bfres;
    using Vibeware.NintenTools.Bfres.Fmdl;
    using Vibeware.NintenTools.Maths;

    /// <summary>
    /// Represents a model converter supporting the OBJ/MTL format.
    /// </summary>
    internal class ObjMtlConverter : ModelConverterBase
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjMtlConverter"/> class with the given <see cref="BfresFile"/>
        ///  and output <see cref="Stream"/>.
        /// </summary>
        /// <param name="bfresFile">The <see cref="BfresFile"/> containing the model data.</param>
        /// <param name="output">The output <see cref="Stream"/>.</param>
        internal ObjMtlConverter(BfresFile bfresFile, FileStream output)
            : base(bfresFile, output)
        {
        }
        
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Converts the BFRES file into the model format.
        /// </summary>
        internal override void Convert()
        {
            using (StreamWriter writer = new StreamWriter(Output))
            {
                // Go through each FmdlSection in the BFRES file.
                foreach (FmdlSection fmdlSection in BfresFile.FmdlSections)
                {
                    // Go through each model in the BFRES file.
                    foreach (FshpModel model in fmdlSection.Models)
                    {
                        ConvertModel(writer, model);
                    }
                }
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void ConvertModel(StreamWriter writer, FshpModel model)
        {
            // Begin a new object and write the corresponding vertex and index data.
            writer.WriteLine("o " + model.Name);
            WriteVertexData(writer, model);
            // TODO: WriteIndexData(writer, model);
        }

        private void WriteVertexData(StreamWriter writer, FshpModel model)
        {
            // There seems to be only one data array in a vertex buffer, so we take it.
            FvtxVertexData vertexData = model.VertexBuffer.Data[0];

            // Get the positions (required), texture coordinates and normals.
            Vector3F[] positions = GetTypedData<Vector3F>(vertexData, "_p0");
            if (positions == null)
            {
                throw new ConverterException(ReturnCode.OutputConversionFailed, "Model without positional data.");
            }
            Vector2F[] textureCoordinates = GetTypedData<Vector2F>(vertexData, "_u0");
            Vector3F[] normals = GetTypedData<Vector3F>(vertexData, "_n0");

            // Write the vertex data.
            foreach (Vector3F position in positions)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "v {0} {1} {2}", position.X, position.Y, position.Z));
            }
            foreach (Vector2F textureCoordinate in textureCoordinates)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "vt {0} {1}", textureCoordinate.X, textureCoordinate.Y));
            }
            foreach (Vector3F normal in normals)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "vn {0} {1} {2}", normal.X, normal.Y, normal.Z));
            }
        }
        private void WriteIndexData(StreamWriter writer, FshpModel model)
        {
            // Enable smoothing for the upcoming surface.
            writer.WriteLine("s 1");

            // Use only the first LOD model, which is the most detailled one, and ignore any visibility groups.
            FshpLodModel lodModel = model.LodModels[0];

            // Go through the indices (wrong)
            ushort[] indices = lodModel.IndexBuffer.Data;
            for (int i = 0; i < indices.Length; i+= 3)
            {
                writer.WriteLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", indices[i], indices[i + 1], indices[i + 2]));
            }
        }

        private T[] GetTypedData<T>(FvtxVertexData vertexData, string attributeName)
        {
            // Get the attribute by name.
            FvtxVertexAttribute attribute = GetAttributeByName(vertexData, attributeName);
            if (attribute == null)
            {
                throw new ConverterException(ReturnCode.OutputConversionFailed,
                    "Attribute {0} not found in model", attributeName);
            }

            switch (attribute.Format)
            {
                case FvtxVertexAttributeFormat.Two_16Bit_Normalized:
                case FvtxVertexAttributeFormat.Two_32Bit_Float:
                    return vertexData.GetAttributeDataAsVector2F(attribute) as T[];
                case FvtxVertexAttributeFormat.Three_10Bit_Signed:
                case FvtxVertexAttributeFormat.Three_32Bit_Float:
                    return vertexData.GetAttributeDataAsVector3F(attribute) as T[];
                default:
                    throw new ConverterException(ReturnCode.OutputConversionFailed,
                        "Attribute format {0} not yet implemented.", attribute.Format);
            }
        }

        private FvtxVertexAttribute GetAttributeByName(FvtxVertexData vertexData, string attributeName)
        {
            foreach (FvtxVertexAttribute attribute in vertexData.Attributes)
            {
                if (attribute.Name == attributeName)
                {
                    return attribute;
                }
            }
            return null;
        }
    }
}
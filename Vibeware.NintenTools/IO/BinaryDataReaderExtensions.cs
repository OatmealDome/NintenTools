namespace Vibeware.NintenTools.IO
{
    using Vibeware.NintenTools.Bfres;
    using Vibeware.NintenTools.Maths;

    /// <summary>
    /// Represents a set of extension methods for the <see cref="BinaryDataReader"/> class, specific to the BFRES
    /// format.
    /// </summary>
    internal static class BinaryDataReaderExtensions
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------
        
        /// <summary>
        /// Reads a <see cref="Vector2F"/> instance from the current position in the base stream, and advances the
        /// position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="Vector2F"/>.</returns>
        internal static Vector2F ReadVector2F(this BinaryDataReader reader)
        {
            return new Vector2F(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads a <see cref="Vector3F"/> instance from the current position in the base stream, and advances the
        /// position of the stream by 12 bytes.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="Vector3F"/>.</returns>
        internal static Vector3F ReadVector3F(this BinaryDataReader reader)
        {
            return new Vector3F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads a <see cref="Vector4F"/> instance from the current position in the base stream, and advances the
        /// position of the stream by 16 bytes.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="Vector4F"/>.</returns>
        internal static Vector4F ReadVector4F(this BinaryDataReader reader)
        {
            return new Vector4F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads a <see cref="Matrix4x3"/> instance from the current position in the base stream, and advances the
        /// position of the stream by 36 bytes.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="Matrix4x3"/>.</returns>
        internal static Matrix4x3 ReadMatrix4x3(this BinaryDataReader reader)
        {
            return new Matrix4x3(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads a <see cref="BfresOffset"/> instance from the current position in the base stream, and advances the
        /// position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="BfresOffset"/>.</returns>
        internal static BfresOffset ReadBfresOffset(this BinaryDataReader reader)
        {
            return new BfresOffset(reader);
        }

        /// <summary>
        /// Reads a specified amount of <see cref="BfresOffset"/> instances from the current position in the base
        /// stream, and advances the position of the stream by 4 bytes multiplied by the amount of instances read.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The amount of <see cref="BfresOffset"/> instances to read.</param>
        /// <returns>The read <see cref="BfresOffset"/> instances.</returns>
        internal static BfresOffset[] ReadBfresOffsets(this BinaryDataReader reader, int count)
        {
            BfresOffset[] offsets = new BfresOffset[count];
            for (int i = 0; i < count; i++)
            {
                offsets[i] = new BfresOffset(reader);
            }
            return offsets;
        }

        /// <summary>
        /// Reads a <see cref="BfresNameOffset"/> instance from the current position in the base stream, and advances
        /// the position of the stream by 4 bytes. A temporary seek is performed to read in the referenced name.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The read <see cref="BfresNameOffset"/>.</returns>
        internal static BfresNameOffset ReadBfresNameOffset(this BinaryDataReader reader)
        {
            return new BfresNameOffset(reader);
        }

        /// <summary>
        /// Reads a specified amount of <see cref="BfresNameOffset"/> instances from the current position in the base
        /// stream, and advances the position of the stream by 4 bytes multiplied by the amount of instances read.
        /// Temporary seeks are performed to read in the referenced names.
        /// </summary>
        /// <param name="reader">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The amount of <see cref="BfresNameOffset"/> instances to read.</param>
        /// <returns>The read <see cref="BfresNameOffset"/> instances.</returns>
        internal static BfresNameOffset[] ReadBfresNameOffsets(this BinaryDataReader reader, int count)
        {
            BfresNameOffset[] offsets = new BfresNameOffset[count];
            for (int i = 0; i < count; i++)
            {
                offsets[i] = new BfresNameOffset(reader);
            }
            return offsets;
        }
    }
}

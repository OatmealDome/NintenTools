using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.IO;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents the loading and saving logic of BYAML files and returns the resulting file structure in dynamic
    /// objects.
    /// </summary>
    public class ByamlFile
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private List<string> _nameArray;
        private List<string> _stringArray;
        private List<ByamlPath> _pathArray;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        private ByamlFile()
        {
        }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Deserializes and returns the dynamic value of the BYAML node read from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to read the data from.</param>
        public static dynamic Load(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Deserializes and returns the dynamic value of the BYAML node read from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        public static dynamic Load(Stream stream)
        {
            ByamlFile byamlFile = new ByamlFile();
            return byamlFile.Read(stream);
        }

        /// <summary>
        /// Serializes the given dynamic value which requires to be an array or dictionary of BYAML compatible values
        /// and stores it in the given file.
        /// </summary>
        /// <param name="stream">The name of the file to store the data in.</param>
        /// <param name="root">The dynamic value becoming the root of the BYAML file. Must be an array or dictionary of
        /// BYAML compatible values.</param>
        public static void Save(string fileName, dynamic root)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream, root);
            }
        }

        /// <summary>
        /// Serializes the given dynamic value which requires to be an array or dictionary of BYAML compatible values
        /// and stores it in the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to store the data in.</param>
        /// <param name="root">The dynamic value becoming the root of the BYAML file. Must be an array or dictionary of
        /// BYAML compatible values.</param>
        public static void Save(Stream stream, dynamic root)
        {
            ByamlFile byamlFile = new ByamlFile();
            byamlFile.Write(stream, root);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private dynamic Read(Stream stream)
        {
            // Open a reader on the given stream.
            using (BinaryDataReader reader = new BinaryDataReader(stream, Encoding.ASCII, true))
            {
                reader.ByteOrder = ByteOrder.BigEndian;

                // Load the header, specifying magic bytes, version and main node offsets.
                if (reader.ReadString(2) != "BY") throw new ByamlException("Invalid BYAML header.");
                if (reader.ReadUInt16() != 0x0001) throw new ByamlException("Unsupported BYAML version.");
                uint nameArrayOffset = reader.ReadUInt32();
                uint stringArrayOffset = reader.ReadUInt32();
                uint pathArrayOffset = reader.ReadUInt32();
                uint rootOffset = reader.ReadUInt32();

                // Read the name array, holding strings referenced by index for the names of other nodes.
                reader.Seek(nameArrayOffset, SeekOrigin.Begin);
                _nameArray = ReadNode(reader);

                // Read the optional string array, holding strings referenced by index in string nodes.
                if (stringArrayOffset != 0)
                {
                    reader.Seek(stringArrayOffset, SeekOrigin.Begin);
                    _stringArray = ReadNode(reader);
                }

                // Read the optional path array, holding paths referenced by index in path nodes.
                if (pathArrayOffset != 0)
                {
                    reader.Seek(pathArrayOffset, SeekOrigin.Begin);
                    _pathArray = ReadNode(reader);
                }

                // Read the root node.
                reader.Seek(rootOffset, SeekOrigin.Begin);
                return ReadNode(reader);
            }
        }

        private dynamic ReadNode(BinaryDataReader reader, ByamlNodeType nodeType = 0)
        {
            // Read the node type if it has not been provided yet.
            bool nodeTypeGiven = nodeType != 0;
            if (!nodeTypeGiven)
            {
                nodeType = (ByamlNodeType)reader.ReadByte();
            }
            if (nodeType >= ByamlNodeType.Array && nodeType <= ByamlNodeType.PathArray)
            {
                // Get the length of arrays.
                long? oldPos = null;
                if (nodeTypeGiven)
                {
                    // If the node type was given, the array value is read from an offset.
                    uint offset = reader.ReadUInt32();
                    oldPos = reader.Position;
                    reader.Seek(offset, SeekOrigin.Begin);
                }
                else
                {
                    reader.Seek(-1);
                }
                int length = (int)(reader.ReadUInt32() & 0x00FFFFFF);
                dynamic value = null;
                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        value = ReadArrayNode(reader, length);
                        break;
                    case ByamlNodeType.Dictionary:
                        value = ReadDictionaryNode(reader, length);
                        break;
                    case ByamlNodeType.StringArray:
                        value = ReadStringArrayNode(reader, length);
                        break;
                    case ByamlNodeType.PathArray:
                        value = ReadPathArrayNode(reader, length);
                        break;
                    default:
                        throw new ByamlException(String.Format("Unknown node type '{0}'.", nodeType));
                }
                // Seek back to the previous position if this was a value positioned at an offset.
                if (oldPos.HasValue)
                {
                    reader.Seek(oldPos.Value, SeekOrigin.Begin);
                }
                return value;
            }
            else
            {
                // Read the following UInt32 which is representing the value directly.
                switch (nodeType)
                {
                    case ByamlNodeType.StringIndex:
                        return _stringArray[reader.ReadInt32()];
                    case ByamlNodeType.PathIndex:
                        return _pathArray[reader.ReadInt32()];
                    case ByamlNodeType.Boolean:
                        return reader.ReadInt32() != 0;
                    case ByamlNodeType.Integer:
                        return reader.ReadInt32();
                    case ByamlNodeType.Float:
                        return reader.ReadSingle();
                    default:
                        throw new ByamlException(String.Format("Unknown node type '{0}'.", nodeType));
                }
            }
        }

        private List<dynamic> ReadArrayNode(BinaryDataReader reader, int length)
        {
            List<dynamic> array = new List<dynamic>(length);

            // Read the element types of the array.
            byte[] nodeTypes = reader.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            reader.Align(4);
            for (int i = 0; i < length; i++)
            {
                array.Add(ReadNode(reader, (ByamlNodeType)nodeTypes[i]));
            }

            return array;
        }

        private Dictionary<string, dynamic> ReadDictionaryNode(BinaryDataReader reader, int length)
        {
            Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint idxAndType = reader.ReadUInt32();
                int nodeNameIndex = (int)(idxAndType >> 8);
                ByamlNodeType nodeType = (ByamlNodeType)(idxAndType & 0x000000FF);
                string nodeName = _nameArray[nodeNameIndex];
                dictionary.Add(nodeName, ReadNode(reader, nodeType));
            }

            return dictionary;
        }

        private List<string> ReadStringArrayNode(BinaryDataReader reader, int length)
        {
            List<string> stringArray = new List<string>(length);

            // Read the element offsets.
            long nodeOffset = reader.Position - 4; // String offsets are relative to the start of node.
            uint[] offsets = reader.ReadUInt32s(length);

            // Read the strings by seeking to their element offset and then back.
            long oldPosition = reader.Position;
            for (int i = 0; i < length; i++)
            {
                reader.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                stringArray.Add(reader.ReadString(BinaryStringFormat.ZeroTerminated));
            }
            reader.Seek(oldPosition, SeekOrigin.Begin);

            return stringArray;
        }

        private List<ByamlPath> ReadPathArrayNode(BinaryDataReader reader, int length)
        {
            List<ByamlPath> pathArray = new List<ByamlPath>(length);

            // Read the element offsets.
            long nodeOffset = reader.Position - 4; // Path offsets are relative to the start of node.
            uint[] offsets = reader.ReadUInt32s(length + 1);

            // Read the paths by seeking to their element offset and then back.
            long oldPosition = reader.Position;
            for (int i = 0; i < length; i++)
            {
                reader.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                int pointCount = (int)((offsets[i + 1] - offsets[i]) / 0x1C);
                pathArray.Add(ReadPath(reader, pointCount));
            }
            reader.Seek(oldPosition, SeekOrigin.Begin);

            return pathArray;
        }

        private ByamlPath ReadPath(BinaryDataReader reader, int length)
        {
            ByamlPath byamlPath = new ByamlPath();
            for (int j = 0; j < length; j++)
            {
                byamlPath.Add(ReadPathPoint(reader));
            }
            return byamlPath;
        }

        private ByamlPathPoint ReadPathPoint(BinaryDataReader reader)
        {
            ByamlPathPoint point = new ByamlPathPoint();
            point.Position = reader.ReadVector3F();
            point.Normal = reader.ReadVector3F();
            point.Unknown = reader.ReadUInt32();
            return point;
        }

        private void Write(Stream stream, dynamic root)
        {
            // Generate the name, string and path array nodes.
            _nameArray = new List<string>();
            _stringArray = new List<string>();
            _pathArray = new List<ByamlPath>();
            CollectNodeArrayContents(root);
            _nameArray.Sort(StringComparer.Ordinal);
            _stringArray.Sort(StringComparer.Ordinal);

            // Open a writer on the given stream.
            using (BinaryDataWriter writer = new BinaryDataWriter(stream, Encoding.ASCII, true))
            {
                writer.ByteOrder = ByteOrder.BigEndian;

                // Write the header, specifying magic bytes, version and main node offsets.
                writer.Write("BY", BinaryStringFormat.NoPrefixOrTermination);
                writer.Write((short)0x0001);
                Offset nameArrayOffset = writer.ReserveOffset();
                Offset stringArrayOffset = writer.ReserveOffset();
                Offset pathArrayOffset = writer.ReserveOffset();
                Offset rootOffset = writer.ReserveOffset();

                // Write the main nodes.
                WriteValueContents(writer, nameArrayOffset, _nameArray);
                if (_stringArray.Count == 0)
                {
                    writer.Write(0);
                }
                else
                {
                    WriteValueContents(writer, stringArrayOffset, _stringArray);
                }
                if (_pathArray.Count == 0)
                {
                    writer.Write(0);
                }
                else
                {
                    WriteValueContents(writer, pathArrayOffset, _pathArray);
                }
                WriteValueContents(writer, rootOffset, root);
            }
        }

        private void CollectNodeArrayContents(dynamic node)
        {
            if (node is string)
            {
                if (!_stringArray.Contains((string)node))
                {
                    _stringArray.Add((string)node);
                }
            }
            else if (node is ByamlPath)
            {
                _pathArray.Add((ByamlPath)node);
            }
            else if (node is IList<dynamic>)
            {
                foreach (dynamic childNode in node)
                {
                    CollectNodeArrayContents(childNode);
                }
            }
            else if (node is IDictionary<string, dynamic>)
            {
                foreach (KeyValuePair<string, dynamic> entry in node)
                {
                    if (!_nameArray.Contains(entry.Key))
                    {
                        _nameArray.Add(entry.Key);
                    }
                    CollectNodeArrayContents(entry.Value);
                }
            }
        }

        private Offset WriteValue(BinaryDataWriter writer, dynamic value)
        {
            // Only reserve and return an offset for the complex value contents, write simple values directly.
            if (value is string)
            {
                WriteStringIndexNode(writer, value);
                return null;
            }
            else if (value is ByamlPath)
            {
                WritePathIndexNode(writer, value);
                return null;
            }
            else if (value is IList<dynamic>)
            {
                return writer.ReserveOffset();
            }
            else if (value is IDictionary<string, dynamic>)
            {
                return writer.ReserveOffset();
            }
            else if (value is bool)
            {
                writer.Write((bool)value ? 1 : 0);
                return null;
            }
            else if (value is int)
            {
                writer.Write((int)value);
                return null;
            }
            else if (value is float)
            {
                writer.Write((float)value);
                return null;
            }
            else
            {
                throw new ByamlException(value);
            }
        }

        private void WriteValueContents(BinaryDataWriter writer, Offset offset, dynamic value)
        {
            // Satisfy the offset to the complex node value which must be 4-byte aligned.
            writer.Align(4);
            offset.Satisfy();

            // Write the value contents.
            if (value is IList<dynamic>)
            {
                WriteArrayNode(writer, value);
            }
            else if (value is IDictionary<string, dynamic>)
            {
                WriteDictionaryNode(writer, value);
            }
            else if (value is IList<string>)
            {
                WriteStringArrayNode(writer, value);
            }
            else if (value is IList<ByamlPath>)
            {
                WritePathArrayNode(writer, value);
            }
            else
            {
                throw new ByamlException(value);
            }
        }

        private void WriteTypeAndLength(BinaryDataWriter writer, dynamic node)
        {
            uint value = (uint)GetNodeType(node) << 24;
            value |= (uint)node.Count;
            writer.Write(value);
        }

        private void WriteStringIndexNode(BinaryDataWriter writer, string node)
        {
            writer.Write(_stringArray.IndexOf(node));
        }

        private void WritePathIndexNode(BinaryDataWriter writer, ByamlPath node)
        {
            writer.Write(_pathArray.IndexOf(node));
        }

        private void WriteArrayNode(BinaryDataWriter writer, IList<dynamic> node)
        {
            WriteTypeAndLength(writer, node);

            // Write the element types.
            foreach (dynamic element in node)
            {
                writer.Write((byte)GetNodeType(element));
            }

            // Write the elements, which begin after a padding to the next 4 bytes.
            writer.Align(4);
            List<Offset> offsets = new List<Offset>(node.Count);
            foreach (dynamic element in node)
            {
                offsets.Add(WriteValue(writer, element));
            }

            // Write the contents of complex nodes and satisfy the offsets.
            for (int i = 0; i < offsets.Count; i++)
            {
                Offset offset = offsets[i];
                if (offset != null)
                {
                    WriteValueContents(writer, offset, node[i]);
                }
            }
        }

        private void WriteDictionaryNode(BinaryDataWriter writer, IDictionary<string, dynamic> node)
        {
            WriteTypeAndLength(writer, node);

            // Dictionaries need to be sorted by key.
            var sortedDict = node.Values.Zip(node.Keys, (Value, Key) => new { Key, Value })
                .OrderBy(x => x.Key).ToList();

            // Write the key-value pairs.
            List<Offset> offsets = new List<Offset>(node.Count);
            foreach (var keyValuePair in sortedDict)
            {
                // Get the index of the key string in the file's name array and write it together with the type.
                uint keyIndex = (uint)_nameArray.IndexOf(keyValuePair.Key);
                writer.Write(keyIndex << 8 | (uint)GetNodeType(keyValuePair.Value));

                // Write the elements.
                offsets.Add(WriteValue(writer, keyValuePair.Value));
            }

            // Write the value contents.
            for (int i = 0; i < offsets.Count; i++)
            {
                Offset offset = offsets[i];
                if (offset != null)
                {
                    WriteValueContents(writer, offset, sortedDict[i].Value);
                }
            }
        }

        private void WriteStringArrayNode(BinaryDataWriter writer, IList<string> node)
        {
            writer.Align(4);
            WriteTypeAndLength(writer, node);

            // Write the offsets to the strings, where the last one points to the end of the last string.
            long offset = 4 + 4 * (node.Count + 1); // Relative to node start + all uint32 offsets.
            foreach (string str in node)
            {
                writer.Write((uint)offset);
                offset += str.Length + 1;
            }
            writer.Write((uint)offset);

            // Write the 0-terminated strings.
            foreach (string str in node)
            {
                writer.Write(str, BinaryStringFormat.ZeroTerminated);
            }
        }

        private void WritePathArrayNode(BinaryDataWriter writer, IList<ByamlPath> node)
        {
            writer.Align(4);
            WriteTypeAndLength(writer, node);

            // Write the offsets to the paths, where the last one points to the end of the last path.
            long offset = 4 + 4 * (node.Count + 1); // Relative to node start + all uint32 offsets.
            foreach (ByamlPath path in node)
            {
                writer.Write((uint)offset);
                offset += path.Count * 28; // 28 bytes are required for a single point.
            }
            writer.Write((uint)offset);

            // Write the paths.
            foreach (ByamlPath path in node)
            {
                WritePathNode(writer, path);
            }
        }

        private void WritePathNode(BinaryDataWriter writer, ByamlPath node)
        {
            foreach (ByamlPathPoint point in node)
            {
                WritePathPoint(writer, point);
            }
        }

        private void WritePathPoint(BinaryDataWriter writer, ByamlPathPoint point)
        {
            writer.Write(point.Position);
            writer.Write(point.Normal);
            writer.Write(point.Unknown);
        }

        private ByamlNodeType GetNodeType(dynamic node)
        {
            if (node is string) return ByamlNodeType.StringIndex;
            else if (node is ByamlPath) return ByamlNodeType.PathIndex;
            else if (node is IList<dynamic>) return ByamlNodeType.Array;
            else if (node is IDictionary<string, dynamic>) return ByamlNodeType.Dictionary;
            else if (node is IList<string>) return ByamlNodeType.StringArray;
            else if (node is IList<ByamlPath>) return ByamlNodeType.PathArray;
            else if (node is bool) return ByamlNodeType.Boolean;
            else if (node is int) return ByamlNodeType.Integer;
            else if (node is float) return ByamlNodeType.Float;
            else throw new ByamlException(node);
        }
    }
}

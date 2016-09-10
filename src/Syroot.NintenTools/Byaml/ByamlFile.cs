using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents the loading and saving logic of BYAML files and returns the resulting file structure.
    /// </summary>
    internal class ByamlFile
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------
        
        private ByamlNode _nameArray;
        private ByamlNode _stringArray;
        private ByamlNode _pathArray;
        
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Deserializes and returns the <see cref="ByamlNode"/> read from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        internal static ByamlNode Load(Stream stream)
        {
            ByamlFile byamlFile = new ByamlFile();
            return byamlFile.Read(stream);
        }

        /// <summary>
        /// Serializes the given <see cref="ByamlNode"/> and stores it in the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to store the data in.</param>
        /// <param name="root">The <see cref="ByamlNode"/> to save.</param>
        /// <param name="includePathArray">If the saved BYAML should have a path table offset.</param>
        internal static void Save(Stream stream, ByamlNode root, bool includePathArray)
        {
            ByamlFile byamlFile = new ByamlFile();
            byamlFile.Write(stream, root, includePathArray);
        }
        
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private ByamlNode Read(Stream stream)
        {
            // Open a reader on the given stream.
            using (BinaryDataReader reader = new BinaryDataReader(stream, Encoding.ASCII, true))
            {
                reader.ByteOrder = ByteOrder.BigEndian;

                // Load the header, specifying magic bytes, version and main node offsets.
                if (reader.ReadString(2) != "BY")
                {
                    throw new ByamlException("Invalid BYAML header.");
                }
                if (reader.ReadUInt16() != 0x0001)
                {
                    throw new ByamlException("Unsupported BYAML version.");
                }
                uint nameArrayOffset = reader.ReadUInt32();
                uint stringArrayOffset = reader.ReadUInt32();
                uint offsetThree = reader.ReadUInt32();
                uint offsetFour = reader.ReadUInt32();

                // Read the name array, holding strings referenced by index for the names of other nodes.
                reader.Seek(nameArrayOffset, SeekOrigin.Begin);
                _nameArray = ReadNode(reader);

                // Read the optional string array, holding strings referenced by index in string nodes.
                if (stringArrayOffset != 0)
                {
                    reader.Seek(stringArrayOffset, SeekOrigin.Begin);
                    _stringArray = ReadNode(reader);
                }

                if (offsetFour > reader.Length)
                {
                    // Splatoon BYAMLs are weird because they don't include a path array offset.
                    reader.Seek(offsetThree, SeekOrigin.Begin);
                    return ReadNode(reader);
                }
                else
                {
                    // This is a more sane BYAML with a path array offset.
                    // Read the optional path array, holding paths referenced by index in path nodes.
                    if (offsetThree != 0)
                    {
                        reader.Seek(offsetThree, SeekOrigin.Begin);
                        _pathArray = ReadNode(reader);
                    }

                    // Read the root node.
                    reader.Seek(offsetFour, SeekOrigin.Begin);
                    return ReadNode(reader);
                }
            }
        }

        private ByamlNode ReadNode(BinaryDataReader reader, ByamlNodeType nodeType = 0)
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
                ByamlNode value = null;
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
                // Read the following UInt32 representing the value directly.
                switch (nodeType)
                {
                    case ByamlNodeType.StringIndex:
                        return new ByamlNode((string)_stringArray[reader.ReadInt32()]);
                    case ByamlNodeType.PathIndex:
                        return new ByamlNode((ByamlPath)_pathArray[reader.ReadInt32()]);
                    case ByamlNodeType.Boolean:
                        return new ByamlNode(reader.ReadInt32() != 0);
                    case ByamlNodeType.Integer:
                        return new ByamlNode(reader.ReadInt32());
                    case ByamlNodeType.Float:
                        return new ByamlNode(reader.ReadSingle());
                    case ByamlNodeType.Null:
                        reader.Seek(4);
                        return new ByamlNode();
                    default:
                        throw new ByamlException(String.Format("Unknown node type '{0}'.", nodeType));
                }
            }
        }

        private ByamlNode ReadArrayNode(BinaryDataReader reader, int length)
        {
            List<ByamlNode> array = new List<ByamlNode>();

            // Read the element types of the array.
            byte[] nodeTypes = reader.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            reader.Align(4);
            for (int i = 0; i < length; i++)
            {
                array.Add(ReadNode(reader, (ByamlNodeType)nodeTypes[i]));
            }

            return new ByamlNode(array);
        }

        private ByamlNode ReadDictionaryNode(BinaryDataReader reader, int length)
        {
            Dictionary<string, ByamlNode> dictionary = new Dictionary<string, ByamlNode>();

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint idxAndType = reader.ReadUInt32();
                int nodeNameIndex = (int)(idxAndType >> 8);
                ByamlNodeType nodeType = (ByamlNodeType)(idxAndType & 0x000000FF);
                string nodeName = (string)_nameArray[nodeNameIndex];
                dictionary.Add(nodeName, ReadNode(reader, nodeType));
            }

            return new ByamlNode(dictionary);
        }

        private ByamlNode ReadStringArrayNode(BinaryDataReader reader, int length)
        {
            List<string> stringArray = new List<string>();

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

            return new ByamlNode(stringArray);
        }

        private ByamlNode ReadPathArrayNode(BinaryDataReader reader, int length)
        {
            List<ByamlPath> pathArray = new List<ByamlPath>();

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

            return new ByamlNode(pathArray);
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

        private void Write(Stream stream, ByamlNode root, bool includePathArray)
        {
            // Generate the name, string and path array nodes.
            _nameArray = new ByamlNode(new List<string>());
            _stringArray = new ByamlNode(new List<string>());
            _pathArray = new ByamlNode(new List<ByamlPath>());
            CollectNodeArrayContents(root);
            _nameArray.Sort(StringComparison.Ordinal);
            _stringArray.Sort(StringComparison.Ordinal);

            // Open a writer on the given stream.
            using (BinaryDataWriter writer = new BinaryDataWriter(stream, Encoding.ASCII, true))
            {
                writer.ByteOrder = ByteOrder.BigEndian;

                // Write the header, specifying magic bytes, version and main node offsets.
                writer.Write("BY", BinaryStringFormat.NoPrefixOrTermination);
                writer.Write((short)0x0001);
                Offset nameArrayOffset = writer.ReserveOffset();
                Offset stringArrayOffset = writer.ReserveOffset();
                Offset offsetThree = null;
                Offset offsetFour = null;

                if (!includePathArray)
                {
                    // This file will not have a path array offset.
                    offsetThree = writer.ReserveOffset();
                }
                else
                {
                    offsetThree = writer.ReserveOffset();
                    offsetFour = writer.ReserveOffset();
                }

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

                if (!includePathArray)
                {
                    WriteValueContents(writer, offsetThree, root);
                }
                else
                {
                    if (_pathArray.Count == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        WriteValueContents(writer, offsetThree, _pathArray);
                    }
                    WriteValueContents(writer, offsetFour, root);
                }
            }
        }

        private void CollectNodeArrayContents(ByamlNode node)
        {
            switch (node.Type)
            {
                case ByamlNodeType.StringIndex:
                    if (!_stringArray.Contains((string)node))
                    {
                        _stringArray.Add((string)node);
                    }
                    break;
                case ByamlNodeType.PathIndex:
                    _pathArray.Add((ByamlPath)node);
                    break;
                case ByamlNodeType.Array:
                    foreach (ByamlNode childNode in node)
                    {
                        CollectNodeArrayContents(childNode);
                    }
                    break;
                case ByamlNodeType.Dictionary:
                    foreach (KeyValuePair<string, ByamlNode> entry in (Dictionary<string, ByamlNode>)node)
                    {
                        if (!_nameArray.Contains(entry.Key))
                        {
                            _nameArray.Add(entry.Key);
                        }
                        CollectNodeArrayContents(entry.Value);
                    }
                    break;
            }
        }

        private Offset WriteValue(BinaryDataWriter writer, ByamlNode value)
        {
            // Only reserve and return an offset for the complex value contents, write simple values directly.
            switch (value.Type)
            {
                case ByamlNodeType.StringIndex:
                    WriteStringIndexNode(writer, value);
                    return null;
                case ByamlNodeType.PathIndex:
                    WritePathIndexNode(writer, value);
                    return null;
                case ByamlNodeType.Array:
                    return writer.ReserveOffset();
                case ByamlNodeType.Dictionary:
                    return writer.ReserveOffset();
                case ByamlNodeType.Boolean:
                    writer.Write((bool)value ? 1 : 0);
                    return null;
                case ByamlNodeType.Integer:
                    writer.Write((int)value);
                    return null;
                case ByamlNodeType.Float:
                    writer.Write((float)value);
                    return null;
                case ByamlNodeType.Null:
                    writer.Write(0);
                    return null;
                default:
                    throw new ByamlNodeTypeException(value.Type);
            }
        }

        private void WriteValueContents(BinaryDataWriter writer, Offset offset, ByamlNode value)
        {
            // Satisfy the offset to the complex node value which must be 4-byte aligned.
            writer.Align(4);
            offset.Satisfy();

            // Write the value contents.
            switch (value.Type)
            {
                case ByamlNodeType.Array:
                    WriteArrayNode(writer, value);
                    break;
                case ByamlNodeType.Dictionary:
                    WriteDictionaryNode(writer, value);
                    break;
                case ByamlNodeType.StringArray:
                    WriteStringArrayNode(writer, value);
                    break;
                case ByamlNodeType.PathArray:
                    WritePathArrayNode(writer, value);
                    break;
                default:
                    throw new ByamlNodeTypeException(value.Type);
            }
        }

        private void WriteTypeAndLength(BinaryDataWriter writer, ByamlNode node)
        {
            uint value = (uint)node.Type << 24;
            value |= (uint)node.Count;
            writer.Write(value);
        }

        private void WriteStringIndexNode(BinaryDataWriter writer, ByamlNode node)
        {
            writer.Write(_stringArray.IndexOf((string)node));
        }

        private void WritePathIndexNode(BinaryDataWriter writer, ByamlNode node)
        {
            writer.Write(_pathArray.IndexOf((ByamlPath)node));
        }

        private void WriteArrayNode(BinaryDataWriter writer, ByamlNode node)
        {
            WriteTypeAndLength(writer, node);

            // Write the element types.
            foreach (ByamlNode element in node)
            {
                writer.Write((byte)element.Type);
            }

            // Write the elements, which begin after a padding to the next 4 bytes.
            writer.Align(4);
            List<Offset> offsets = new List<Offset>();
            foreach (ByamlNode element in node)
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

        private void WriteDictionaryNode(BinaryDataWriter writer, ByamlNode node)
        {
            WriteTypeAndLength(writer, node);

            // Dictionaries need to be sorted by key.
            var sortedDict = node.Values.Zip(node.Keys, (Value, Key) => new { Key, Value })
                .OrderBy(x => x.Key).ToList();

            // Write the key-value pairs.
            List<Offset> offsets = new List<Offset>();
            foreach (var keyValuePair in sortedDict)
            {
                // Get the index of the key string in the file's name array and write it together with the type.
                uint keyIndex = (uint)_nameArray.IndexOf(keyValuePair.Key);
                writer.Write(keyIndex << 8 | (uint)keyValuePair.Value.Type);

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

        private void WriteStringArrayNode(BinaryDataWriter writer, ByamlNode node)
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

        private void WritePathArrayNode(BinaryDataWriter writer, ByamlNode node)
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

        private void WritePathNode(BinaryDataWriter writer, ByamlNode node)
        {
            foreach (ByamlPathPoint point in (ByamlPath)node)
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
    }
}

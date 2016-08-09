using System;
using System.IO;
using System.Text;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents the loading logic to parse BYAML files and returns the resulting file structure.
    /// </summary>
    internal class ByamlLoader
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlLoader"/> class, reading the data from the given stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        internal ByamlLoader(Stream stream)
        {
            // Open a reader on the given stream.
            using (Reader = new BinaryDataReader(stream, Encoding.ASCII, true))
            {
                Reader.ByteOrder = ByteOrder.BigEndian;

                // Load the header, specifying magic bytes, version and main node information.
                if (Reader.ReadString(2) != "BY")
                {
                    throw new ByamlException("Invalid BYAML header.");
                }
                if (Reader.ReadUInt16() != 0x0001)
                {
                    throw new ByamlException("Unsupported BYAML version.");
                }
                uint nameArrayOffset = Reader.ReadUInt32();
                uint stringArrayOffset = Reader.ReadUInt32();
                uint pathArrayOffset = Reader.ReadUInt32();
                uint rootOffset = Reader.ReadUInt32();

                // Read the name array, holding strings referenced by index for the names of other nodes.
                Reader.Seek(nameArrayOffset, SeekOrigin.Begin);
                NameArray = LoadNode();

                // Read the optional string array, holding strings referenced by index in string nodes.
                if (stringArrayOffset != 0)
                {
                    Reader.Seek(stringArrayOffset, SeekOrigin.Begin);
                    StringArray = LoadNode();
                }

                // Read the optional path array, holding paths referenced by index in path nodes.
                if (pathArrayOffset != 0)
                {
                    Reader.Seek(pathArrayOffset, SeekOrigin.Begin);
                    PathArray = LoadNode();
                }

                // Read the root node.
                Reader.Seek(rootOffset, SeekOrigin.Begin);
                Root = LoadNode();
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="BinaryDataReader"/> which provides access to the raw data.
        /// </summary>
        internal BinaryDataReader Reader { get; private set; }

        /// <summary>
        /// Gets the name array, holding string referenced by index for the name of other nodes.
        /// </summary>
        internal ByamlNode NameArray { get; private set; }

        /// <summary>
        /// Gets the optional string array, holding strings referenced by index in string nodes.
        /// </summary>
        internal ByamlNode StringArray { get; private set; }

        /// <summary>
        /// Gets the optional path array, holding paths referenced by index in path nodes.
        /// </summary>
        internal ByamlNode PathArray { get; private set; }

        /// <summary>
        /// Gets the root <see cref="ByamlNode"/> of the file structure.
        /// </summary>
        internal ByamlNode Root { get; private set; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Loads a <see cref="ByamlNode"/> from the current position with the given type if it is known already.
        /// </summary>
        /// <param name="nodeType">The <see cref="ByamlNodeType"/> of the node, if known already.</param>
        /// <returns>The loaded <see cref="ByamlNode"/> instance.</returns>
        internal ByamlNode LoadNode(ByamlNodeType nodeType = 0)
        {
            // Read the node type if it has not been provided yet.
            bool nodeTypeGiven = nodeType != 0;
            if (!nodeTypeGiven)
            {
                nodeType = (ByamlNodeType)Reader.ReadByte();
            }
            if (nodeType >= ByamlNodeType.Array && nodeType <= ByamlNodeType.PathArray)
            {
                // Get the length of arrays.
                long? oldPos = null;
                if (nodeTypeGiven)
                {
                    // If the node type was given, the array value is read from an offset.
                    uint offset = Reader.ReadUInt32();
                    oldPos = Reader.Position;
                    Reader.Seek(offset, SeekOrigin.Begin);
                }
                else
                {
                    Reader.Seek(-1);
                }
                int length = (int)(Reader.ReadUInt32() & 0x00FFFFFF);
                ByamlNode value = null;
                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        value = new ByamlNode(ByamlNodeType.Array, this, length);
                        break;
                    case ByamlNodeType.Dictionary:
                        value = new ByamlNode(ByamlNodeType.Dictionary, this, length);
                        break;
                    case ByamlNodeType.StringArray:
                        value = new ByamlNode(ByamlNodeType.StringArray, this, length);
                        break;
                    case ByamlNodeType.PathArray:
                        value = new ByamlNode(ByamlNodeType.PathArray, this, length);
                        break;
                    default:
                        throw new ByamlException(String.Format("Unknown node type '{0}'.", nodeType));
                }
                // Seek back to the previous position if this was a value positioned at an offset.
                if (oldPos.HasValue)
                {
                    Reader.Seek(oldPos.Value, SeekOrigin.Begin);
                }
                return value;
            }
            else
            {
                // Read the following UInt32 representing the value directly.
                switch (nodeType)
                {
                    case ByamlNodeType.StringIndex:
                        return new ByamlNode(ByamlNodeType.StringIndex, this);
                    case ByamlNodeType.PathIndex:
                        return new ByamlNode(ByamlNodeType.PathIndex, this);
                    case ByamlNodeType.Boolean:
                        return new ByamlNode(ByamlNodeType.Boolean, this);
                    case ByamlNodeType.Integer:
                        return new ByamlNode(ByamlNodeType.Integer, this);
                    case ByamlNodeType.Float:
                        return new ByamlNode(ByamlNodeType.Float, this);
                    default:
                        throw new ByamlException(String.Format("Unknown node type '{0}'.", nodeType));
                }
            }
        }
    }
}

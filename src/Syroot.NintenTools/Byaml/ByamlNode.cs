using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents a node in a BYAML file. This can either be a collection of child nodes or a final value.
    /// </summary>
    /// <remarks>This is more or less a direct port of the Python API. The code might not be optimal for the CLR due to
    /// the dynamic approach used in Python.</remarks>
    [DebuggerDisplay("{Type}, {ToString()}")]
    public class ByamlNode : IList<ByamlNode>, IEquatable<ByamlNode>, ICloneable
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private List<string>    _keys;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private List<ByamlNode> _values;

        private string    _string;
        private ByamlPath _path;
        private bool?     _boolean;
        private int?      _integer;
        private float?    _float;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(string value)
        {
            Type = ByamlNodeType.StringIndex;
            _string = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(ByamlPath value)
        {
            Type = ByamlNodeType.PathIndex;
            _path = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(List<ByamlNode> value)
        {
            Type = ByamlNodeType.Array;
            _values = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(Dictionary<string, ByamlNode> value)
        {
            Type = ByamlNodeType.Dictionary;

            _keys = new List<string>();
            _values = new List<ByamlNode>();
            foreach (KeyValuePair<string, ByamlNode> keyValuePair in value)
            {
                _keys.Add(keyValuePair.Key);
                _values.Add(keyValuePair.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(List<string> value)
        {
            Type = ByamlNodeType.StringArray;

            _values = new List<ByamlNode>();
            foreach (string element in value)
            {
                _values.Add(new ByamlNode(element));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(List<ByamlPath> value)
        {
            Type = ByamlNodeType.PathArray;

            _values = new List<ByamlNode>();
            foreach (ByamlPath element in value)
            {
                _values.Add(new ByamlNode(element));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(bool value)
        {
            Type = ByamlNodeType.Boolean;
            _boolean = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(int value)
        {
            Type = ByamlNodeType.Integer;
            _integer = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        public ByamlNode(float value)
        {
            Type = ByamlNodeType.Float;
            _float = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class with node type Null.
        /// </summary>
        public ByamlNode()
        {
            Type = ByamlNodeType.Null;
        }

        /// <summary>
        /// Gets the <see cref="ByamlNode"/> at the given index.
        /// </summary>
        /// <param name="index">The index of the <see cref="ByamlNode"/> to retrieve.</param>
        /// <returns>The <see cref="ByamlNode"/> at the given index.</returns>
        public ByamlNode this[int index]
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Array:
                    case ByamlNodeType.StringArray:
                    case ByamlNodeType.PathArray:
                        return _values[index];
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.Array:
                    case ByamlNodeType.StringArray:
                    case ByamlNodeType.PathArray:
                        _values[index] = value;
                        break;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ByamlNode"/> with the given key.
        /// </summary>
        /// <param name="key">The key of the <see cref="ByamlNode"/> to retrieve.</param>
        /// <returns>The <see cref="ByamlNode"/> at the given index.</returns>
        public ByamlNode this[string key]
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        for (int i = 0; i < _keys.Count; i++)
                        {
                            if (_keys[i] == key)
                            {
                                return _values[i];
                            }
                        }
                        throw new KeyNotFoundException();
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        for (int i = 0; i < _keys.Count; i++)
                        {
                            if (_keys[i] == key)
                            {
                                _values[i] = value;
                                return;
                            }
                        }
                        throw new KeyNotFoundException();
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public ByamlNodeType Type;

        /// <summary>
        /// Gets the number of elements in the collection node.
        /// </summary>
        public int Count
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Array:
                    case ByamlNodeType.Dictionary:
                    case ByamlNodeType.StringArray:
                    case ByamlNodeType.PathArray:
                        return _values.Count;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection node is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys of the dictionary node.
        /// </summary>
        public List<string> Keys
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        return _keys;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values of the dictionary node.
        /// </summary>
        public List<ByamlNode> Values
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        return _values;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }
        
        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(string value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(ByamlPath value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(List<ByamlNode> value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(Dictionary<string, ByamlNode> value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(List<string> value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(List<ByamlPath> value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(bool value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(int value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets a <see cref="ByamlNode"/> out of the value.
        /// </summary>
        /// <param name="value">The value the <see cref="ByamlNode"/> will have.</param>
        public static implicit operator ByamlNode(float value)
        {
            return new ByamlNode(value);
        }

        /// <summary>
        /// Gets the <see cref="String"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator string(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.StringIndex)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._string;
        }

        /// <summary>
        /// Gets the <see cref="ByamlPath"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator ByamlPath(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.PathIndex)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._path;
        }

        /// <summary>
        /// Gets the <see cref="List{ByamlNode}"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator List<ByamlNode>(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.Array)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._values;
        }

        /// <summary>
        /// Gets the <see cref="Dictionary{string, ByamlNode}"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator Dictionary<string, ByamlNode>(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.Dictionary)
            {
                throw new ByamlNodeTypeException(node.Type);
            }

            Dictionary<string, ByamlNode> dictionary = new Dictionary<string, ByamlNode>();
            for (int i = 0; i < node._values.Count; i++)
            {
                dictionary.Add(node._keys[i], node._values[i]);
            }
            return dictionary;
        }

        /// <summary>
        /// Gets the <see cref="List{string}"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator List<string>(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.StringArray)
            {
                throw new ByamlNodeTypeException(node.Type);
            }

            return node._values.ConvertAll(x => (string)x);
        }
        
        /// <summary>
        /// Gets the <see cref="List{ByamlPath}"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator List<ByamlPath>(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.PathArray)
            {
                throw new ByamlNodeTypeException(node.Type);
            }

            return node._values.ConvertAll(x => (ByamlPath)x);
        }

        /// <summary>
        /// Gets the <see cref="ByamlPath"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator bool(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.Boolean)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._boolean.Value;
        }

        /// <summary>
        /// Gets the <see cref="ByamlPath"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator int(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.Integer)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._integer.Value;
        }

        /// <summary>
        /// Gets the <see cref="Single"/> value of this node.
        /// </summary>
        /// <param name="node">The node which value will be retrieved.</param>
        public static explicit operator float(ByamlNode node)
        {
            if (node.Type != ByamlNodeType.Float)
            {
                throw new ByamlNodeTypeException(node.Type);
            }
            return node._float.Value;
        }
        
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Loads a new instance of the <see cref="ByamlNode"/> class from the given stream.
        /// </summary>
        /// <param name="fileName">The name of the file to read the contents from.</param>
        /// <returns>The new <see cref="ByamlNode"/> instance.</returns>
        public static ByamlNode Load(Stream stream)
        {
            return ByamlFile.Load(stream);
        }

        /// <summary>
        /// Loads a new instance of the <see cref="ByamlNode"/> class from the file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file to read the contents from.</param>
        /// <returns>The new <see cref="ByamlNode"/> instance.</returns>
        public static ByamlNode Load(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the given stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents in.</param>
        /// <param name="includePathArray">If the saved BYAML should have a path table offset.</param>
        public void Save(Stream stream, bool includePathArray)
        {
            if (Type != ByamlNodeType.Array && Type != ByamlNodeType.Dictionary)
            {
                throw new ByamlNodeTypeException("Only Array or Dictionary nodes can be saved as the BYAML root.");
            }

            ByamlFile.Save(stream, this, includePathArray);
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file to save the contents in.</param>
        /// <param name="includePathArray">If the saved BYAML should have a path table offset.</param>
        public void Save(string fileName, bool includePathArray)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream, includePathArray);
            }
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the given stream. A path table offset will be included.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents in.</param>
        public void Save(Stream stream)
        {
            Save(stream, true);
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the file with the given name. A path table offset will be included.
        /// </summary>
        /// <param name="fileName">The name of the file to save the contents in.</param>
        public void Save(string fileName)
        {
            Save(fileName, true);
        }

        /// <summary>
        /// Adds an item to the collection node.
        /// </summary>
        /// <param name="item">The object to add to the collection node.</param>
        public void Add(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _values.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item with the specific key to the collection node.
        /// </summary>
        /// <param name="key">The key under which the item will be added.</param>
        /// <param name="item">The object to add to the collection node.</param>
        public void Add(string key, ByamlNode value)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    if (_keys.Contains(key))
                    {
                        throw new ArgumentException(String.Format("An item with the key '{0}' has already been added.",
                            key));
                    }
                    _keys.Add(key);
                    _values.Add(value);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the collection node.
        /// </summary>
        /// <param name="item">The object to add to the collection node.</param>
        public void Add(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    if (_keys.Contains(item.Key))
                    {
                        throw new ArgumentException(String.Format("An item with the key '{0}' has already been added.",
                            item.Key));
                    }
                    _keys.Add(item.Key);
                    _values.Add(item.Value);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the collection node.
        /// </summary>
        /// <param name="item">The object to add to the collection node.</param>
        public void Add(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _values.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the collection node.
        /// </summary>
        /// <param name="item">The object to add to the collection node.</param>
        public void Add(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    _values.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes all items from the collection node.
        /// </summary>
        public void Clear()
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                case ByamlNodeType.StringArray:
                case ByamlNodeType.PathArray:
                    _values.Clear();
                    break;
                case ByamlNodeType.Dictionary:
                    _keys.Clear();
                    _values.Clear();
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the collection node contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>true if <paramref name="item"/> is found in the collection node; otherwise, false.</returns>
        public bool Contains(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _values.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the collection node contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>true if <paramref name="item"/> is found in the collection node; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    return _keys.Contains(item.Key) && _values.Contains(item.Value);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the collection node contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>true if <paramref name="item"/> is found in the collection node; otherwise, false.</returns>
        public bool Contains(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _values.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the collection node contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>true if <paramref name="item"/> is found in the collection node; otherwise, false.</returns>
        public bool Contains(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _values.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the collection node contains a specific key.
        /// </summary>
        /// <param name="key">The name of the key to locate in the collection node.</param>
        /// <returns>true if <paramref name="key"/> is found in the collection node; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    return _keys.Contains(key);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Copies the elements of the collection node to an <see cref="Array"/>, starting at a particular
        /// <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from collection node. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(ByamlNode[] array, int arrayIndex)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _values.CopyTo(array, arrayIndex);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ByamlNode other)
        {
            if (other == null || Type != other.Type)
            {
                return false;
            }
            switch (Type)
            {
                case ByamlNodeType.StringIndex:
                    return _string == other._string;
                case ByamlNodeType.PathIndex:
                    return _path == other._path;
                case ByamlNodeType.Array:
                case ByamlNodeType.StringArray:
                case ByamlNodeType.PathArray:
                    return _values.SequenceEqual(other._values);
                case ByamlNodeType.Dictionary:
                    return _keys.SequenceEqual(other._keys) && _values.SequenceEqual(other._values);
                case ByamlNodeType.Boolean:
                    return _boolean == other._boolean;
                case ByamlNodeType.Integer:
                    return _integer == other._integer;
                case ByamlNodeType.Float:
                    return _float == other._float;
                case ByamlNodeType.Null:
                    return true;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ByamlNode> GetEnumerator()
        {
            return _values.GetEnumerator();
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the collection node.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _values.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the collection node.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _values.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the collection node.
        /// </summary>
        /// <param name="item">The object to locate in the collection node.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _values.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the collection node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection node.</param>
        public void Insert(int index, ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _values.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the collection node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection node.</param>
        public void Insert(int index, string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _values.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the collection node at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection node.</param>
        public void Insert(int index, ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    _values.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection node.
        /// </summary>
        /// <param name="item">The object to remove from the collection node.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the collection node; otherwise,
        /// false. This method also returns false if <paramref name="item"/> is not found in the original collection
        /// node.</returns>
        public bool Remove(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _values.Remove(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection node.
        /// </summary>
        /// <param name="item">The object to remove from the collection node.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the collection node; otherwise,
        /// false. This method also returns false if <paramref name="item"/> is not found in the original collection
        /// node.</returns>
        public bool Remove(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (_keys[i] == item.Key)
                        {
                            if (_values[i] == item.Value)
                            {

                                _keys.RemoveAt(i);
                                _values.RemoveAt(i);
                                return true;
                            }
                            return false;
                        }
                    }
                    return false;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object or value with the given key from the collection node.
        /// </summary>
        /// <param name="item">The object or key of the value to remove from the collection node.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the collection node; otherwise,
        /// false. This method also returns false if <paramref name="item"/> is not found in the original collection
        /// node.</returns>
        public bool Remove(string itemOrKey)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (_keys[i] == itemOrKey)
                        {
                            _keys.RemoveAt(i);
                            _values.RemoveAt(i);
                            return true;
                        }
                    }
                    return false;
                case ByamlNodeType.StringArray:
                    return _values.Remove(itemOrKey);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection node.
        /// </summary>
        /// <param name="item">The object to remove from the collection node.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the collection node; otherwise,
        /// false. This method also returns false if <paramref name="item"/> is not found in the original collection
        /// node.</returns>
        public bool Remove(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _values.Remove(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the collection node item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                case ByamlNodeType.StringArray:
                case ByamlNodeType.PathArray:
                    _values.RemoveAt(index);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        public void Sort(StringComparison comparisonType)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _values.Sort((x, y) => String.Compare((string)x, (string)y, comparisonType));
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Tries to return the value under the given key.
        /// </summary>
        /// <param name="key">The key which value should be retrieved.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>true if the value was found; otherwise, false.</returns>
        public bool TryGetValue(string key, out ByamlNode value)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (_keys[i] == key)
                        {
                            value = _values[i];
                            return true;
                        }
                    }
                    value = null;
                    return false;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        public Object Clone()
        {
            switch (Type)
            {
                case ByamlNodeType.StringIndex:
                    return new ByamlNode((String)_string.Clone());
                case ByamlNodeType.PathIndex:
                    return new ByamlNode((ByamlPath)_path.Clone());
                case ByamlNodeType.Array:
                    List<ByamlNode> nodeList = new List<ByamlNode>();
                    foreach (ByamlNode node in _values)
                    {
                        nodeList.Add((ByamlNode)node.Clone());
                    }

                    return new ByamlNode(nodeList);
                case ByamlNodeType.Dictionary:
                    Dictionary<string, ByamlNode> dictionary = new Dictionary<string, ByamlNode>();
                    foreach (String key in _keys)
                    {
                        dictionary.Add((String)key.Clone(), (ByamlNode)this[key].Clone());
                    }

                    return new ByamlNode(dictionary);
                case ByamlNodeType.StringArray:
                    List<String> strings = new List<String>();
                    foreach (String str in _values)
                    {
                        strings.Add((String)str.Clone());
                    }

                    return new ByamlNode(strings);
                case ByamlNodeType.PathArray:
                    List<ByamlPath> pathArray = new List<ByamlPath>();
                    foreach (ByamlPath path in _values)
                    {
                        pathArray.Add((ByamlPath)path.Clone());
                    }

                    return new ByamlNode(pathArray);
                case ByamlNodeType.Boolean:
                    return new ByamlNode(_boolean ?? default(bool));
                case ByamlNodeType.Integer:
                    return new ByamlNode(_integer ?? default(int));
                case ByamlNodeType.Float:
                    return new ByamlNode(_float ?? default(float));
                case ByamlNodeType.Null:
                    return new ByamlNode();
            }

            throw new ByamlNodeTypeException();
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="String" /> that represents this instance.</returns>
        public override string ToString()
        {
            switch (Type)
            {
                case ByamlNodeType.StringIndex:
                    return _string;
                case ByamlNodeType.PathIndex:
                    return _path.ToString();
                case ByamlNodeType.Array:
                case ByamlNodeType.Dictionary:
                case ByamlNodeType.StringArray:
                case ByamlNodeType.PathArray:
                    return "Count = " + _values.Count.ToString();
                case ByamlNodeType.Boolean:
                    return _boolean.ToString();
                case ByamlNodeType.Integer:
                    return _integer.ToString();
                case ByamlNodeType.Float:
                    return _float.ToString();
                case ByamlNodeType.Null:
                    return "(null)";
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }
    }
}

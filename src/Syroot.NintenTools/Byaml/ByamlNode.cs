using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Syroot.NintenTools.IO;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents a node in a <see cref="ByamlFile"/>. This can either be a collection of nodes itself or a final
    /// value.
    /// </summary>
    public class ByamlNode : IList<ByamlNode>, IDictionary<string, ByamlNode>, IList<string>, IList<ByamlPath>
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private string _string;
        private ByamlPath _path;
        private List<ByamlNode> _array;
        private Dictionary<string, ByamlNode> _dictionary;
        private List<string> _stringArray;
        private List<ByamlPath> _pathArray;
        private bool _boolean;
        private int _integer;
        private float _float;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(string value)
        {
            Type = ByamlNodeType.StringIndex;
            _string = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(ByamlPath value)
        {
            Type = ByamlNodeType.PathIndex;
            _path = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(List<ByamlNode> value)
        {
            Type = ByamlNodeType.Array;
            _array = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(Dictionary<string, ByamlNode> value)
        {
            Type = ByamlNodeType.Dictionary;
            _dictionary = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(List<string> value)
        {
            Type = ByamlNodeType.StringArray;
            _stringArray = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(List<ByamlPath> value)
        {
            Type = ByamlNodeType.PathArray;
            _pathArray = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(bool value)
        {
            Type = ByamlNodeType.Boolean;
            _boolean = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(int value)
        {
            Type = ByamlNodeType.Integer;
            _integer = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class from the given value.
        /// </summary>
        /// <param name="value">The value the node will have.</param>
        internal ByamlNode(float value)
        {
            Type = ByamlNodeType.Float;
            _float = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class, loading data according to the given
        /// <see cref="ByamlNodeType"/> with the provided <see cref="ByamlLoader"/>.
        /// </summary>
        /// <param name="type">The type of the node data to read.</param>
        /// <param name="loader">The <see cref="ByamlLoader"/> to load data with.</param>
        internal ByamlNode(ByamlNodeType type, ByamlLoader loader)
        {
            Type = type;
            switch (Type)
            {
                case ByamlNodeType.StringIndex:
                    _string = (string)loader.StringArray[loader.Reader.ReadInt32()];
                    break;
                case ByamlNodeType.PathIndex:
                    _path = (ByamlPath)loader.PathArray[loader.Reader.ReadInt32()];
                    break;
                case ByamlNodeType.Boolean:
                    _boolean = loader.Reader.ReadInt32() != 0;
                    break;
                case ByamlNodeType.Integer:
                    _integer = loader.Reader.ReadInt32();
                    break;
                case ByamlNodeType.Float:
                    _float = loader.Reader.ReadSingle();
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNode"/> class for array nodes with the specified length,
        /// loading data according to the given <see cref="ByamlNodeType"/> with the provided <see cref="ByamlLoader"/>.
        /// </summary>
        /// <param name="type">The type of the node data to read.</param>
        /// <param name="loader">The <see cref="ByamlLoader"/> to load data with.</param>
        /// <param name="length">The number of entries in the array node.</param>
        internal ByamlNode(ByamlNodeType type, ByamlLoader loader, int length)
        {
            Type = type;
            switch (Type)
            {
                case ByamlNodeType.Array:
                    LoadArray(loader, length);
                    break;
                case ByamlNodeType.Dictionary:
                    LoadDictionary(loader, length);
                    break;
                case ByamlNodeType.StringArray:
                    LoadStringArray(loader, length);
                    break;
                case ByamlNodeType.PathArray:
                    LoadPathArray(loader, length);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public ByamlNodeType Type { get; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection"/>.
        /// </summary>
        public int Count
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Array:
                        return _array.Count;
                    case ByamlNodeType.Dictionary:
                        return _dictionary.Count;
                    case ByamlNodeType.StringArray:
                        return _stringArray.Count;
                    case ByamlNodeType.PathArray:
                        return _pathArray.Count;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection"/>.
        /// </summary>
        int ICollection<string>.Count
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.StringArray:
                        return _stringArray.Count;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection"/>.
        /// </summary>
        int ICollection<ByamlPath>.Count
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.PathArray:
                        return _pathArray.Count;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys of the <see cref="IDictionary"/>.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        return _dictionary.Keys;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values in the <see cref="IDictionary"/>.
        /// </summary>
        public ICollection<ByamlNode> Values
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        return _dictionary.Values;
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
            return node._array;
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
            return node._dictionary;
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
            return node._stringArray;
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
            return node._pathArray;
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
            return node._boolean;
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
            return node._integer;
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
            return node._float;
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
                        return _array[index];
                    case ByamlNodeType.StringArray:
                        return _stringArray[index];
                    case ByamlNodeType.PathArray:
                        return _pathArray[index];
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.Array:
                        _array[index] = value;
                        break;
                    case ByamlNodeType.StringArray:
                        _stringArray[index] = (string)value;
                        break;
                    case ByamlNodeType.PathArray:
                        _pathArray[index] = (ByamlPath)value;
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
                        return _dictionary[key];
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.Dictionary:
                        _dictionary[key] = value;
                        break;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="String"/> at the specified index.
        /// </summary>
        string IList<string>.this[int index]
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.StringArray:
                        return _stringArray[index];
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.StringArray:
                        _stringArray[index] = value;
                        break;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ByamlPath"/> at the specified index.
        /// </summary>
        ByamlPath IList<ByamlPath>.this[int index]
        {
            get
            {
                switch (Type)
                {
                    case ByamlNodeType.PathArray:
                        return _pathArray[index];
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
            set
            {
                switch (Type)
                {
                    case ByamlNodeType.PathArray:
                        _pathArray[index] = value;
                        break;
                    default:
                        throw new ByamlNodeTypeException(Type);
                }
            }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Loads a new instance of the <see cref="ByamlNode"/> class from the given stream.
        /// </summary>
        /// <param name="fileName">The name of the file to read the contents from.</param>
        /// <returns>The new <see cref="ByamlNode"/> instance.</returns>
        public static ByamlNode Load(Stream stream)
        {
            ByamlLoader loader = new ByamlLoader(stream);
            return loader.Root;
        }

        /// <summary>
        /// Loads a new instance of the <see cref="ByamlNode"/> class from the file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file to read the contents from.</param>
        /// <returns>The new <see cref="ByamlNode"/> instance.</returns>
        public static ByamlNode Load(string fileName)
        {
            return Load(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the given stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents in.</param>
        public void Save(Stream stream)
        {
            if (Type != ByamlNodeType.Array && Type != ByamlNodeType.Dictionary)
            {
                throw new ByamlNodeTypeException("Only Array or Dictionary nodes can be saved as BYAML data.");
            }

            // TODO
        }

        /// <summary>
        /// Stores this node as a new BYAML file in the file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file to save the contents in.</param>
        public void Save(string fileName)
        {
            Save(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None));
        }

        /// <summary>
        /// Exposes the enumerator, which supports a simple iteration over a collection of <see cref="ByamlNode"/>
        /// instances.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ByamlNode> GetEnumerator()
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _array.GetEnumerator();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _stringArray.GetEnumerator();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<ByamlPath> IEnumerable<ByamlPath>.GetEnumerator()
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _pathArray.GetEnumerator();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<string, ByamlNode>> IEnumerable<KeyValuePair<string, ByamlNode>>.GetEnumerator()
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    return _dictionary.GetEnumerator();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        public void Add(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _array.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, ByamlNode value)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    _dictionary.Add(key, value);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        public void Add(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    _dictionary.Add(item.Key, item.Value);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        public void Add(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _stringArray.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        public void Add(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    _pathArray.Add(item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection"/>.
        /// </summary>
        public void Clear()
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _stringArray.Clear();
                    break;
                case ByamlNodeType.PathArray:
                    _pathArray.Clear();
                    break;
                case ByamlNodeType.Array:
                    _array.Clear();
                    break;
                case ByamlNodeType.Dictionary:
                    _dictionary.Clear();
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> is found in the <see cref="ICollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _array.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> is found in the <see cref="ICollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    throw new NotImplementedException();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> is found in the <see cref="ICollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _stringArray.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> is found in the <see cref="ICollection"/>; otherwise, false.
        /// </returns>
        public bool Contains(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _pathArray.Contains(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary"/>.</param>
        /// <returns>true if the <see cref="IDictionary"/> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    return _dictionary.ContainsKey(key);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular
        /// <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(ByamlNode[] array, int arrayIndex)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _array.CopyTo(array, arrayIndex);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular
        /// <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, ByamlNode>[] array, int arrayIndex)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    throw new NotImplementedException();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular
        /// <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _stringArray.CopyTo(array, arrayIndex);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular
        /// <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(ByamlPath[] array, int arrayIndex)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    _pathArray.CopyTo(array, arrayIndex);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _array.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _stringArray.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _pathArray.IndexOf(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList"/>.</param>
        public void Insert(int index, ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    _array.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList"/>.</param>
        public void Insert(int index, string item)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _stringArray.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList"/>.</param>
        public void Insert(int index, ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    _pathArray.Insert(index, item);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the <see cref="ICollection"/>;
        /// otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original
        /// <see cref="ICollection"/>.</returns>
        public bool Remove(ByamlNode item)
        {
            switch (Type)
            {
                case ByamlNodeType.Array:
                    return _array.Remove(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the <see cref="ICollection"/>;
        /// otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original
        /// <see cref="ICollection"/>.</returns>
        public bool Remove(KeyValuePair<string, ByamlNode> item)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    throw new NotImplementedException();
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/> or removes the element
        /// with the specified key from the <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="itemOrKey">The object to remove from the <see cref="ICollection"/> or the key of the element to
        /// remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if
        /// <paramref name="itemOrKey"/> was not found in the original <see cref="ICollection"/> or
        /// <see cref="IDictionary"/>.</returns>
        public bool Remove(string itemOrKey)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    return _stringArray.Remove(itemOrKey);
                case ByamlNodeType.Dictionary:
                    return _dictionary.Remove(itemOrKey);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection"/>.</param>
        /// <returns>true if <paramref name="item"/> was successfully removed from the <see cref="ICollection"/>;
        /// otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original
        /// <see cref="ICollection"/>.</returns>
        public bool Remove(ByamlPath item)
        {
            switch (Type)
            {
                case ByamlNodeType.PathArray:
                    return _pathArray.Remove(item);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Removes the <see cref="IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            switch (Type)
            {
                case ByamlNodeType.StringArray:
                    _stringArray.RemoveAt(index);
                    break;
                case ByamlNodeType.PathArray:
                    _pathArray.RemoveAt(index);
                    break;
                case ByamlNodeType.Array:
                    _array.RemoveAt(index);
                    break;
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is
        /// found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter
        /// is passed uninitialized.</param>
        /// <returns>true if the object that implements <see cref="IDictionary"/> contains an element with the
        /// specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out ByamlNode value)
        {
            switch (Type)
            {
                case ByamlNodeType.Dictionary:
                    return _dictionary.TryGetValue(key, out value);
                default:
                    throw new ByamlNodeTypeException(Type);
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void LoadArray(ByamlLoader loader, int length)
        {
            _array = new List<ByamlNode>();

            // Read the element types of the array.
            byte[] nodeTypes = loader.Reader.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            loader.Reader.Align(4);
            for (int i = 0; i < length; i++)
            {
                _array.Add(loader.LoadNode((ByamlNodeType)nodeTypes[i]));
            }
        }

        private void LoadDictionary(ByamlLoader loader, int length)
        {
            _dictionary = new Dictionary<string, ByamlNode>();

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint idxAndType = loader.Reader.ReadUInt32();
                int nodeNameIndex = (int)(idxAndType >> 8);
                ByamlNodeType nodeType = (ByamlNodeType)(idxAndType & 0x000000FF);
                string nodeName = (string)loader.NameArray[nodeNameIndex];
                _dictionary.Add(nodeName, loader.LoadNode(nodeType));
            }
        }

        private void LoadStringArray(ByamlLoader loader, int length)
        {
            _stringArray = new List<string>();

            // Read the element offsets.
            long nodeOffset = loader.Reader.Position - 4; // String offsets are relative to the start of node.
            uint[] offsets = loader.Reader.ReadUInt32s(length);

            // Read the strings by seeking to their element offset and then back.
            long oldPosition = loader.Reader.Position;
            for (int i = 0; i < length; i++)
            {
                loader.Reader.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                _stringArray.Add(loader.Reader.ReadString(BinaryStringFormat.ZeroTerminated));
            }
            loader.Reader.Seek(oldPosition, SeekOrigin.Begin);
        }

        private void LoadPathArray(ByamlLoader loader, int length)
        {
            _pathArray = new List<ByamlPath>();

            // Read the element offsets.
            long nodeOffset = loader.Reader.Position - 4; // Path offsets are relative to the start of node.
            uint[] offsets = loader.Reader.ReadUInt32s(length + 1);

            // Read the paths by seeking to their element offset and then back.
            long oldPosition = loader.Reader.Position;
            for (int i = 0; i < length; i++)
            {
                loader.Reader.Seek(nodeOffset + offsets[i]);
                int pointCount = (int)((offsets[i + 1] - offsets[i]) / 0x1C);
                _pathArray.Add(new ByamlPath(loader.Reader, pointCount));
            }
            loader.Reader.Seek(oldPosition, SeekOrigin.Begin);
        }
    }
}

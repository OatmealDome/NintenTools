namespace Vibeware.NintenTools.Bfres
{
    using System.Diagnostics;
    using Vibeware.NintenTools.IO;

    /// <summary>
    /// Represents a node in a <see cref="BfresIndexGroup"/>.
    /// </summary>
    [DebuggerDisplay("Index Group {Name}")]
    public class BfresIndexGroupNode
    {
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit a <see cref="BfresIndexGroupNode"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the index leading to the left node, which is computed by an unknown method.
            /// </summary>
            public ushort LeftNodeIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index leading to the right node, which is computed by an unknown method.
            /// </summary>
            public ushort RightNodeIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the offset to the name of this entry.
            /// </summary>
            public BfresNameOffset NameOffset
            {
                get;
                set;
            }
        }

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BfresIndexGroupNode"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal BfresIndexGroupNode(BfresLoaderContext context)
        {
            Load(context);
            // LeftNode and RightNode are filled by parent index group.
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="BfresIndexGroupNode"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value used for the binary search tree traversal algorithm.
        /// </summary>
        public uint SearchValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the entry.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the left node.
        /// </summary>
        public BfresIndexGroupNode LeftNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right node.
        /// </summary>
        public BfresIndexGroupNode RightNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset of the data this entry references.
        /// </summary>
        public BfresOffset DataPointer
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            SearchValue = context.Reader.ReadUInt32();
            Internal.LeftNodeIndex = context.Reader.ReadUInt16();
            Internal.RightNodeIndex = context.Reader.ReadUInt16();
            Internal.NameOffset = context.Reader.ReadBfresNameOffset();
            Name = Internal.NameOffset.Name;
            DataPointer = context.Reader.ReadBfresOffset();
        }
    }
}

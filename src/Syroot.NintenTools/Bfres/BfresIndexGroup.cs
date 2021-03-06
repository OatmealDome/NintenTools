﻿namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a binary search tree to quickly look up elements by name.
    /// </summary>
    public class BfresIndexGroup
    {
        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit a <see cref="BfresIndexGroup"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets the length of the group in bytes.
            /// </summary>
            public uint Length
            {
                get;
                set;
            }
        }

        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresIndexGroup"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal BfresIndexGroup(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="BfresIndexGroup"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of <see cref="BfresIndexGroupNode"/> instances without the root node.
        /// </summary>
        public uint NodeCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the array of all <see cref="BfresIndexGroupNode"/> instances, not organized by hierarchy. The first
        /// node does not reference real data and is just a reference node, thus, real data nodes start at index 1.
        /// </summary>
        public BfresIndexGroupNode[] Nodes
        {
            get;
            set;
        }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="BfresIndexGroupNode"/> with the given name or <c>null</c> if no node has that name.
        /// </summary>
        /// <param name="nodeName">The name of the node to retrieve.</param>
        /// <returns>The node with the given name or <c>null</c> if no node has this name.</returns>
        public BfresIndexGroupNode this[string nodeName]
        {
            get
            {
                foreach (BfresIndexGroupNode node in Nodes)
                {
                    if (node.Name == nodeName)
                    {
                        return node;
                    }
                }

                return null;
            }
        }
        
        /// <summary>
        /// Gets the <see cref="BfresIndexGroupNode"/> with the given index, as it has been load from the file.
        /// </summary>
        /// <param name="index">The index of the node to retrieve.</param>
        /// <returns>The node with the given index.</returns>
        public BfresIndexGroupNode this[int index]
        {
            get
            {
                return Nodes[index];
            }
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the index of the node with the given name or <c>null</c> if no node has that name.
        /// </summary>
        /// <param name="nodeName">The name of the node to retrieve.</param>
        /// <returns>The index of the node with the given name or <c>null</c> if no node has this name.</returns>
        public int? GetIndexByName(string nodeName)
        {
            // It is useless to check the reference node since the name is always empty, but maybe the user wants that.
            for (int i = 0; i < Nodes.Length; i++)
            {
                BfresIndexGroupNode node = Nodes[i];
                if (node.Name == nodeName)
                {
                    return i;
                }
            }

            return null;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.Length = context.Reader.ReadUInt32();
            NodeCount = context.Reader.ReadUInt32();

            // Read the nodes (first node is no actual data, but reference point, and doesn't count).
            Nodes = new BfresIndexGroupNode[(int)NodeCount + 1];
            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i] = new BfresIndexGroupNode(context);
            }
        }
    }
}

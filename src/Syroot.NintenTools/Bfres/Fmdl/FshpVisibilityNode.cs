namespace Syroot.NintenTools.Bfres.Fmdl
{
    using System;
    using System.Collections.Generic;
    using Syroot.NintenTools.Maths;

    /// <summary>
    /// Represents a node in the visibility group binary tree of a <see cref="FmdlSection"/>.
    /// </summary>
    public class FshpVisibilityNode
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FshpVisibilityNode"/> class from the given
        /// <see cref="BfresLoaderContext"/>. The reader of the context has to be positioned at the start of the data.
        /// </summary>
        /// <param name="context">The loader context providing information about how to load the data.</param>
        internal FshpVisibilityNode(BfresLoaderContext context)
        {
            Load(context);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets internal data not required to edit a <see cref="FshpVisibilityNode"/> instance.
        /// </summary>
        public Internals Internal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the left child node.
        /// </summary>
        public FshpVisibilityNode LeftNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right child node.
        /// </summary>
        public FshpVisibilityNode RightNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown position associated with this node.
        /// </summary>
        public Vector3F UnknownPosition1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an unknown position associated with this node.
        /// </summary>
        public Vector3F UnknownPosition2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of referenced visibility groups. It is unknown how these are referenced from a
        /// <see cref="FshpLodModel"/>.
        /// </summary>
        public List<FshpVisibilityGroup> VisibilityGroups
        {
            get;
            set;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void Load(BfresLoaderContext context)
        {
            Internal = new Internals();

            Internal.LeftNodeIndex = context.Reader.ReadUInt16();
            Internal.RightNodeIndex = context.Reader.ReadUInt16();
            Internal.Unknown0x04 = context.Reader.ReadUInt16();
            Internal.SiblingNodeIndex = context.Reader.ReadUInt16();
            Internal.VisibilityGroupIndex = context.Reader.ReadUInt16();
            Internal.VisibilityGroupCount = context.Reader.ReadUInt16();
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Represents internal data not required to edit an <see cref="FshpVisibilityNode"/> instance.
        /// </summary>
        public class Internals
        {
            // ---- PROPERTIES -----------------------------------------------------------------------------------------

            /// <summary>
            /// Gets or sets the index of the left child, or the node's own index if it has no left child.
            /// </summary>
            public ushort LeftNodeIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the right child, or the node's own index if it has no right child.
            /// </summary>
            public ushort RightNodeIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an unknown variable at offset 0x04. The value always seems to be the same as
            /// <see cref="LeftNodeIndex"/>.
            /// </summary>
            public ushort Unknown0x04
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the next sibling node. For left nodes, this is the parent's right node. If the
            /// parent has no right node, it is this nodes own index.
            /// </summary>
            public ushort SiblingNodeIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the first of <see cref="FshpVisibilityGroup"/> instances to reference in the corresponding
            /// <see cref="FshpLodModel"/>.
            /// </summary>
            public ushort VisibilityGroupIndex
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of <see cref="FshpVisibilityGroup"/> instances to reference in the corresponding
            /// <see cref="FshpLodModel"/>, beginning from the first group as determined by
            /// <see cref="VisibilityGroupIndex"/>.
            /// </summary>
            public ushort VisibilityGroupCount
            {
                get;
                set;
            }
        }
    }
}
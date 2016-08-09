using System;
using System.Runtime.Serialization;

namespace Syroot.NintenTools.Byaml
{
    /// <summary>
    /// Represents errors that occur when trying to execute operations of <see cref="ByamlNode"/> instances which are
    /// not supported due to their <see cref="ByamlNodeType"/>.
    /// </summary>
    [Serializable]
    public class ByamlNodeTypeException : Exception
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNodeTypeException"/> class.
        /// </summary>
        public ByamlNodeTypeException()
            : base("Node type does not support operation.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNodeTypeException"/> class with the specified node type
        /// which did not support the operation.
        /// </summary>
        public ByamlNodeTypeException(ByamlNodeType type)
            : base(String.Format("Node type '{0}' does not support operation.", type))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNodeTypeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ByamlNodeTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNodeTypeException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner"> The exception that is the cause of the current exception, or a null reference if no
        /// inner exception is specified.</param>
        public ByamlNodeTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlNodeTypeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the
        /// exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the
        /// source or destination.</param>
        protected ByamlNodeTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

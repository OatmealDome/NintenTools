using System;
using System.Globalization;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an exception caused by invalid BFRES data.
    /// </summary>
    public class BfresException : Exception
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresException"/> class.
        /// </summary>
        public BfresException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresException"/> class with the given message.
        /// </summary>
        /// <param name="message">The message provided with the exception.</param>
        public BfresException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresException"/> class with the given message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message provided with the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public BfresException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfresException"/> class with the given message.
        /// </summary>
        /// <param name="format">A composite format string representing the message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public BfresException(string format, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, format, args))
        {
        }
    }
}

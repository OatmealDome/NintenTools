namespace Syroot.NintenTools.BfresConverter
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception caused by conversion problems.
    /// </summary>
    [Serializable]
    public class ConverterException : Exception
    {
        // ---- CONSTRUCTORS -------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class.
        /// </summary>
        public ConverterException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class with the given message.
        /// </summary>
        /// <param name="message">The message provided with the exception.</param>
        public ConverterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class with the given message and inner
        /// exception.
        /// </summary>
        /// <param name="message">The message provided with the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConverterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class with the return code and message.
        /// </summary>
        /// <param name="returnCode">A <see cref="ReturnCode"/> value to return from the main program.</param>
        /// <param name="format">A composite format string representing the message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public ConverterException(ReturnCode returnCode)
            : base(returnCode.GetDefaultMessage())
        {
            ReturnCode = returnCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class with the return code and additional
        /// detail message.
        /// </summary>
        /// <param name="returnCode">A <see cref="ReturnCode"/> value to return from the main program.</param>
        /// <param name="format">A composite format string representing the message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public ConverterException(ReturnCode returnCode, string format, params object[] args)
            : base(returnCode.GetDefaultMessage() + ": " + string.Format(CultureInfo.CurrentCulture, format, args))
        {
            ReturnCode = returnCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the
        /// exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult"/> is zero
        /// (0).</exception>
        protected ConverterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets the <see cref="ReturnCode"/> value to return from the main program.
        /// </summary>
        public ReturnCode ReturnCode
        {
            get;
            private set;
        }
    }
}

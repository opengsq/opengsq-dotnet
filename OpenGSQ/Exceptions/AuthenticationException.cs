using System;

namespace OpenGSQ.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}

using System;

namespace OpenGSQ
{
    /// <summary>
    /// Represents errors that occur during application execution when a packet is invalid.
    /// </summary>
    public class InvalidPacketException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPacketException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidPacketException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Represents errors that occur during application execution when authentication fails.
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthenticationException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Represents errors that occur during application execution when a server is not found.
    /// </summary>
    public class ServerNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerNotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServerNotFoundException(string message) : base(message)
        {

        }
    }
}

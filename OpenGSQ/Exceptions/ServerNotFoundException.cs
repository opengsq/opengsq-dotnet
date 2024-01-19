using System;

namespace OpenGSQ.Exceptions
{
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

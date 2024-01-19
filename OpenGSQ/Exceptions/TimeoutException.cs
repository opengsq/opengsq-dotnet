using System;

namespace OpenGSQ.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution related to timeouts.
    /// </summary>
    public class TimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TimeoutException(string message) : base(message)
        {
        }
    }
}

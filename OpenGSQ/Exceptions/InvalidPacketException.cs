using System;
using System.Linq;

namespace OpenGSQ.Exceptions
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

        /// <summary>
        /// Checks if the received value is equal to the expected value.
        /// </summary>
        /// <typeparam name="T">The type of the values to compare.</typeparam>
        /// <param name="received">The received value.</param>
        /// <param name="expected">The expected value.</param>
        /// <exception cref="InvalidPacketException">
        /// Thrown when the received value does not match the expected value.
        /// </exception>
        public static void ThrowIfNotEqual<T>(T received, T expected)
        {
            if (typeof(T) == typeof(byte[]))
            {
                if (!(received as byte[])!.SequenceEqual((expected as byte[])!))
                {
                    throw new InvalidPacketException(GetMessage(received, expected));
                }
            }
            else if (!received!.Equals(expected))
            {
                throw new InvalidPacketException(GetMessage(received, expected));
            }
        }

        private static string GetMessage<T>(T received, T expected)
        {
            string receivedStr;
            string expectedStr;

            if (typeof(T) == typeof(byte[]))
            {
                receivedStr = BitConverter.ToString((received as byte[])!);
                expectedStr = BitConverter.ToString((expected as byte[])!);
            }
            else
            {
                receivedStr = received!.ToString()!;
                expectedStr = expected!.ToString()!;
            }

            return $"Packet header mismatch. Received: {receivedStr}. Expected: {expectedStr}.";
        }
    }
}

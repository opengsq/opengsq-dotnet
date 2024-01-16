using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenGSQ
{
    /// <summary>
    /// Provides extension methods for the BinaryReader class.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Determines whether the end of the stream has been reached.
        /// </summary>
        /// <param name="br">The BinaryReader instance on which the extension method is called.</param>
        /// <returns>true if the end of the stream is reached; otherwise, false.</returns>
        public static bool IsEnd(this BinaryReader br)
        {
            return br.BaseStream.Position >= br.BaseStream.Length;
        }

        /// <summary>
        /// Reads a string from the current stream until charByte.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="charBytes"></param>
        /// <returns>The string being read.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static string ReadStringEx(this BinaryReader br, byte[] charBytes)
        {
            charBytes ??= new byte[] { 0 };

            var bytes = new List<byte>();
            byte streamByte;

            while (Array.IndexOf(charBytes, streamByte = br.ReadByte()) == -1)
            {
                bytes.Add(streamByte);
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Reads a string from the current stream until charByte.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="charByte"></param>
        /// <returns>The string being read.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static string ReadStringEx(this BinaryReader br, byte charByte = 0)
        {
            return br.ReadStringEx(new byte[] { charByte });
        }

        /// <summary>
        /// Reads a string from the current stream until charByte. Return true if is not null and empty.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="outString"></param>
        /// <param name="charBytes"></param>
        /// <returns></returns>
        public static bool TryReadStringEx(this BinaryReader br, out string outString, byte[] charBytes)
        {
            outString = br.ReadStringEx(charBytes);

            return !string.IsNullOrEmpty(outString);
        }

        /// <summary>
        /// Reads a string from the current stream until charByte. Return true if is not null and empty.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="outString"></param>
        /// <param name="charByte"></param>
        /// <returns></returns>
        public static bool TryReadStringEx(this BinaryReader br, out string outString, byte charByte = 0)
        {
            return br.TryReadStringEx(out outString, new byte[] { charByte });
        }
    }
}

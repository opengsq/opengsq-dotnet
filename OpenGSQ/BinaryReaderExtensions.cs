using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenGSQ
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a string from the current stream until charByte.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="charByte"></param>
        /// <returns>The string being read.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static string ReadStringEx(this BinaryReader br, byte charByte = 0x00)
        {
            var bytes = new List<byte>();
            byte streamByte;

            while ((streamByte = br.ReadByte()) != charByte)
            {
                bytes.Add(streamByte);
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}

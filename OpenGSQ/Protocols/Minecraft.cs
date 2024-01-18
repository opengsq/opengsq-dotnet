using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Minecraft Protocol (https://wiki.vg/Server_List_Ping)
    /// </summary>
    public class Minecraft : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Minecraft Protocol";

        /// <summary>
        /// Initializes a new instance of the Minecraft class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout. Default is 5000.</param>
        public Minecraft(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the server status asynchronously.
        /// </summary>
        /// <param name="version">The protocol version. Default is 47.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server status.</returns>
        public async Task<Dictionary<string, object>> GetStatus(int version = 47)
        {
            // Prepare the request
            var address = Encoding.UTF8.GetBytes(Host);
            var protocol = PackVarint(version);
            var request = new byte[] { 0x00 }.Concat(protocol).Concat(PackVarint(address.Length)).Concat(address).Concat(BitConverter.GetBytes((short)Port)).Concat(new byte[] { 0x01 }).ToArray();
            request = PackVarint(request.Length).Concat(request).Concat(new byte[] { 0x01, 0x00 }).ToArray();

            using (var tcpClient = new System.Net.Sockets.TcpClient())
            {
                tcpClient.ReceiveTimeout = Timeout;
                await tcpClient.ConnectAsync(Host, Port);
                await tcpClient.SendAsync(request);

                byte[] response = await tcpClient.ReceiveAsync();

                using (var br = new BinaryReader(new MemoryStream(response)))
                {
                    var length = UnpackVarint(br);

                    // Keep receiving until reach packet length
                    while (response.Length < length)
                    {
                        response = response.Concat(await tcpClient.ReceiveAsync()).ToArray();
                    }
                }

                // Read full response
                using (var br = new BinaryReader(new MemoryStream(response)))
                {
                    UnpackVarint(br);  // packet length
                    UnpackVarint(br);  // packet id
                    var count = UnpackVarint(br);  // json length

                    // The packet may respond with two json objects, so we need to get the json length exactly
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(Encoding.UTF8.GetString(br.ReadBytes(count)));

                    return data;
                }
            }
        }

        /// <summary>
        /// Gets the server status for servers using a version older than Minecraft 1.7.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server status.</returns>
        public async Task<Dictionary<string, object>> GetStatusPre17()
        {
            byte[] response = await TcpClient.CommunicateAsync(this, new byte[] { 0xFE, 0x01 });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadByte();

                if (header != 0xFF)
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {0xFF}.");
                }

                br.ReadBytes(2);  // length of the following string
                var items = Encoding.BigEndianUnicode.GetString(br.ReadBytes(response.Length - 2)).Split('\0');

                var result = new Dictionary<string, object>
                {
                    ["protocol"] = items[1],
                    ["version"] = items[2],
                    ["motd"] = items[3],
                    ["numplayers"] = int.Parse(items[4]),
                    ["maxplayers"] = int.Parse(items[5])
                };

                return result;
            }
        }

        /// <summary>
        /// Strips color codes from the input text.
        /// </summary>
        /// <param name="text">The text to strip color codes from.</param>
        /// <returns>The text with color codes stripped.</returns>
        public static string StripColors(string text)
        {
            // Strip color codes
            return Regex.Replace(text, @"\u00A7[0-9A-FK-OR]", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Packs an integer into a Varint.
        /// </summary>
        /// <param name="val">The integer to pack.</param>
        /// <returns>The packed Varint as a byte array.</returns>
        protected byte[] PackVarint(int val)
        {
            var total = new List<byte>();

            if (val < 0)
            {
                val = (1 << 32) + val;
            }

            while (val >= 0x80)
            {
                var bits = val & 0x7F;
                val >>= 7;
                total.Add((byte)(0x80 | bits));
            }

            var lastBits = val & 0x7F;
            total.Add((byte)lastBits);

            return total.ToArray();
        }

        /// <summary>
        /// Unpacks a Varint into an integer.
        /// </summary>
        /// <param name="br">The BinaryReader to read the Varint from.</param>
        /// <returns>The unpacked integer.</returns>
        protected int UnpackVarint(BinaryReader br)
        {
            var total = 0;
            var shift = 0;
            var val = 0x80;

            while ((val & 0x80) != 0)
            {
                val = br.ReadByte();
                total |= (val & 0x7F) << shift;
                shift += 7;
            }

            if ((total & (1 << 31)) != 0)
            {
                total -= 1 << 32;
            }

            return total;
        }
    }
}

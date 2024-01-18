using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Doom3 Protocol
    /// </summary>
    public class Doom3 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Doom3 Protocol";

        private static readonly Dictionary<string, List<string>> _playerFields = new Dictionary<string, List<string>>()
        {
            ["doom"] = new List<string> { "id", "ping", "rate", "name" },
            ["quake4"] = new List<string> { "id", "ping", "rate", "name", "clantag" },
            ["etqw"] = new List<string> { "id", "ping", "name", "clantag_pos", "clantag", "typeflag" }
        };

        /// <summary>
        /// Initializes a new instance of the Doom3 class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The connection timeout in milliseconds.</param>
        public Doom3(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets information from the server.
        /// </summary>
        /// <param name="stripColor">Whether to strip color codes from the returned information.</param>
        /// <returns>A dictionary containing the server information.</returns>
        public async Task<Dictionary<string, object>> GetInfo(bool stripColor = true)
        {
            byte[] request = new byte[] { 0xFF, 0xFF, 0x67, 0x65, 0x74, 0x49, 0x6E, 0x66, 0x6F, 0x00, 0x6F, 0x67, 0x73, 0x71, 0x00 };
            byte[] response = await UdpClient.CommunicateAsync(this, request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                br.ReadBytes(2); // Skip 2 bytes

                string header = br.ReadStringEx();

                if (header != "infoResponse")
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: infoResponse.");
                }

                // Read challenge
                br.ReadBytes(4);

                if (!br.ReadBytes(4).SequenceEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }))
                {
                    br.BaseStream.Position -= 4;
                }

                Dictionary<string, object> info = new Dictionary<string, object>();

                // Read protocol version
                ushort minor = br.ReadUInt16();
                ushort major = br.ReadUInt16();
                info["version"] = $"{major}.{minor}";

                // Read packet size
                if (br.ReadInt32() != br.RemainingBytes())
                {
                    br.BaseStream.Position -= 4;
                }

                // Key / value pairs, delimited by an empty pair
                while (br.BaseStream.Length > 0)
                {
                    string key = br.ReadStringEx().Trim();
                    string val = br.ReadStringEx().Trim();

                    if (key == "" && val == "")
                    {
                        break;
                    }

                    info[key] = stripColor ? StripColors(val) : val;
                }

                long streamPosition = br.BaseStream.Position;

                // Try parse the fields
                foreach (string mod in _playerFields.Keys)
                {
                    try
                    {
                        info["players"] = ParsePlayer(br, _playerFields[mod], stripColor);
                        break;
                    }
                    catch (Exception)
                    {
                        info["players"] = new List<object>();
                        br.BaseStream.Position = streamPosition;
                    }
                }

                return info;
            }
        }

        private static List<Dictionary<string, object>> ParsePlayer(BinaryReader br, List<string> fields, bool stripColor)
        {
            List<Dictionary<string, object>> players = new List<Dictionary<string, object>>();

            while (true)
            {
                Dictionary<string, object> player = new Dictionary<string, object>();

                foreach (string field in fields)
                {
                    if (field == "id" || field == "clantag_pos" || field == "typeflag")
                    {
                        player[field] = br.ReadByte();
                    }
                    else if (field == "ping")
                    {
                        player[field] = br.ReadInt16();
                    }
                    else if (field == "rate")
                    {
                        player[field] = br.ReadInt32();
                    }
                    else
                    {
                        string str = br.ReadStringEx();
                        player[field] = stripColor ? StripColors(str) : str;
                    }
                }

                if ((byte)player["id"] == 32)
                {
                    break;
                }

                players.Add(player);
            }

            return players;
        }

        /// <summary>
        /// Strips color codes from the input text.
        /// </summary>
        /// <param name="text">The text to strip color codes from.</param>
        /// <returns>The text with color codes removed.</returns>
        public static string StripColors(string text)
        {
            return Regex.Replace(text, "\\^(X.{6}|.)", "");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// All-Seeing Eye Protocol
    /// </summary>
    public class ASE : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "All-Seeing Eye Protocol";

        private readonly byte[] _request = Encoding.ASCII.GetBytes("s");
        private readonly byte[] _response = Encoding.ASCII.GetBytes("EYE1");

        /// <summary>
        /// Initializes a new instance of the <see cref="ASE"/> class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The timeout for the connection.</param>
        public ASE(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        /// <returns>A dictionary containing the server status.</returns>
        public async Task<Dictionary<string, object>> GetStatus()
        {
            using var udpClient = new UdpClient();
            byte[] response = await udpClient.CommunicateAsync(this, _request);
            byte[] header = response[..4];

            if (!header.SequenceEqual(_response))
            {
                throw new InvalidPacketException($"Packet header mismatch. Received: {BitConverter.ToString(header)}. Expected: {BitConverter.ToString(_response)}.");
            }

            using var br = new BinaryReader(new MemoryStream(response[4..]), Encoding.UTF8);

            var result = new Dictionary<string, object>
            {
                ["gamename"] = ReadString(br),
                ["gameport"] = ReadString(br),
                ["hostname"] = ReadString(br),
                ["gametype"] = ReadString(br),
                ["map"] = ReadString(br),
                ["version"] = ReadString(br),
                ["password"] = ReadString(br),
                ["numplayers"] = ReadString(br),
                ["maxplayers"] = ReadString(br),
                ["rules"] = ParseRules(br),
                ["players"] = ParsePlayers(br)
            };

            return result;
        }

        private Dictionary<string, string> ParseRules(BinaryReader br)
        {
            var rules = new Dictionary<string, string>();

            while (!br.IsEnd())
            {
                string key = ReadString(br);

                if (string.IsNullOrEmpty(key))
                {
                    break;
                }

                rules[key] = ReadString(br);
            }

            return rules;
        }

        private List<Dictionary<string, string>> ParsePlayers(BinaryReader br)
        {
            var players = new List<Dictionary<string, string>>();
            var keys = new Dictionary<int, string>
            {
                [1] = "name",
                [2] = "team",
                [4] = "skin",
                [8] = "score",
                [16] = "ping",
                [32] = "time"
            };

            while (!br.IsEnd())
            {
                byte flags = br.ReadByte();
                var player = new Dictionary<string, string>();

                foreach (var key in keys)
                {
                    if ((flags & key.Key) == key.Key)
                    {
                        player[key.Value] = ReadString(br);
                    }
                }

                players.Add(player);
            }

            return players;
        }

        private string ReadString(BinaryReader br)
        {
            int length = br.ReadByte();
            return Encoding.UTF8.GetString(br.ReadBytes(length - 1));
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGSQ.Responses.Unreal2;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Unreal 2 Protocol
    /// </summary>
    public class Unreal2 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Unreal 2 Protocol";

        /// <summary>
        /// Represents the byte value for details.
        /// </summary>
        protected const byte _DETAILS = 0x00;

        /// <summary>
        /// Represents the byte value for rules.
        /// </summary>
        protected const byte _RULES = 0x01;

        /// <summary>
        /// Represents the byte value for players.
        /// </summary>
        protected const byte _PLAYERS = 0x02;

        /// <summary>
        /// Initializes a new instance of the Unreal2 class.
        /// </summary>
        public Unreal2(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the details of the server.
        /// </summary>
        /// <returns>The details of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        public async Task<Status> GetDetails()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, _DETAILS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();

                if (header != _DETAILS)
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {_DETAILS}.");
                }

                var details = new Status
                {
                    ServerId = br.ReadInt32(),
                    ServerIP = br.ReadString(),
                    GamePort = br.ReadInt32(),
                    QueryPort = br.ReadInt32(),
                    ServerName = ReadString(br),
                    MapName = ReadString(br),
                    GameType = ReadString(br),
                    NumPlayers = br.ReadInt32(),
                    MaxPlayers = br.ReadInt32(),
                    Ping = br.ReadInt32(),
                    Flags = br.ReadInt32(),
                    Skill = ReadString(br)
                };

                return details;
            }
        }

        /// <summary>
        /// Gets the rules of the server.
        /// </summary>
        /// <returns>The rules of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        public async Task<Dictionary<string, object>> GetRules()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, _RULES });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();

                if (header != _RULES)
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {_RULES}.");
                }

                var rules = new Dictionary<string, object>();
                var mutators = new List<string>();

                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    string key = ReadString(br);
                    string val = ReadString(br);

                    if (key.ToLower() == "mutator")
                    {
                        mutators.Add(val);
                    }
                    else
                    {
                        rules[key] = val;
                    }
                }

                rules["Mutators"] = mutators;

                return rules;
            }
        }

        /// <summary>
        /// Gets the players of the server.
        /// </summary>
        /// <returns>A list of players of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        public async Task<List<Player>> GetPlayers()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, _PLAYERS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();

                if (header != _PLAYERS)
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {_PLAYERS}.");
                }

                var players = new List<Player>();

                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    var player = new Player
                    {
                        Id = br.ReadInt32(),
                        Name = ReadString(br),
                        Ping = br.ReadInt32(),
                        Score = br.ReadInt32(),
                        StatsId = br.ReadInt32()
                    };

                    players.Add(player);
                }

                return players;
            }
        }

        /// <summary>
        /// Strips color codes from the given text.
        /// </summary>
        /// <param name="text">The text to strip color codes from, represented as a byte array.</param>
        /// <returns>The text with color codes stripped, represented as a string.</returns>
        protected static string StripColors(byte[] text)
        {
            string str = Encoding.UTF8.GetString(text);
            return Regex.Replace(str, @"\x1b...|[\x00-\x1a]", "");
        }

        /// <summary>
        /// Reads a string from a BinaryReader, decodes it, and strips color codes.
        /// </summary>
        /// <param name="br">The BinaryReader to read the string from.</param>
        /// <returns>The decoded string with color codes stripped.</returns>
        protected string ReadString(BinaryReader br)
        {
            int length = br.ReadByte();

            if (length == 0)
            {
                return string.Empty;
            }

            string str = br.ReadStringEx();

            byte[] b;
            if (length == str.Length + 1)
            {
                b = Encoding.UTF8.GetBytes(str);
            }
            else
            {
                b = Encoding.Unicode.GetBytes(str);
            }

            return StripColors(b);
        }
    }
}
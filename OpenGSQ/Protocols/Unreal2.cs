using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;
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
        protected const byte DETAILS = 0x00;

        /// <summary>
        /// Represents the byte value for rules.
        /// </summary>
        protected const byte RULES = 0x01;

        /// <summary>
        /// Represents the byte value for players.
        /// </summary>
        protected const byte PLAYERS = 0x02;

        /// <summary>
        /// Initializes a new instance of the Unreal2 class.
        /// </summary>
        public Unreal2(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Asynchronously retrieves the details of the server.
        /// </summary>
        /// <param name="stripColor">A boolean value indicating whether to strip color codes from the server name. Default is true.</param>
        /// <returns>A <see cref="Task{Status}"/> representing the server status, including details such as server ID, IP, ports, server name, map name, game type, player count, max players, ping, flags, and skill level.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected DETAILS header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetDetails(bool stripColor = true)
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, DETAILS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, DETAILS);

                return new Status
                {
                    ServerId = br.ReadInt32(),
                    ServerIP = br.ReadString(),
                    GamePort = br.ReadInt32(),
                    QueryPort = br.ReadInt32(),
                    ServerName = ReadString(br, stripColor),
                    MapName = ReadString(br),
                    GameType = ReadString(br),
                    NumPlayers = br.ReadInt32(),
                    MaxPlayers = br.ReadInt32(),
                    Ping = br.ReadInt32(),
                    Flags = br.ReadInt32(),
                    Skill = ReadString(br)
                };
            }
        }

        /// <summary>
        /// Gets the rules of the server.
        /// </summary>
        /// <returns>The rules of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Dictionary<string, object>> GetRules()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, RULES });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, RULES);

                var rules = new Dictionary<string, object>();
                var mutators = new List<string>();

                while (!br.IsEnd())
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
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<List<Player>> GetPlayers()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, PLAYERS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, PLAYERS);

                var players = new List<Player>();

                while (!br.IsEnd())
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
        /// Strips color codes from the input text.
        /// </summary>
        /// <param name="text">The input text which may contain color codes.</param>
        /// <returns>A string with color codes stripped out.</returns>
        public static string StripColor(string text)
        {
            return Regex.Replace(text, @"\x1b...|[\x00-\x1a]", "");
        }

        /// <summary>
        /// Reads a string from a BinaryReader, decodes it, and strips color codes.
        /// </summary>
        /// <param name="br">The BinaryReader to read the string from.</param>
        /// <returns>The decoded string with color codes stripped.</returns>
        protected string ReadString(BinaryReader br, bool stripColor = false)
        {
            int length = br.ReadByte();

            string result;
            if (length >= 128)
            {
                length = (length & 0x7f) * 2;
                byte[] bytes = br.ReadBytes(length);
                result = Encoding.Unicode.GetString(bytes);
            }
            else
            {
                byte[] bytes = br.ReadBytes(length);
                result = Encoding.UTF8.GetString(bytes);
            }

            result = stripColor ? StripColor(result) : result.TrimEnd();

            if (!br.IsEnd() && br.ReadByte() != 0)
            {
                br.BaseStream.Position -= 1;
            }

            return StripColor(result);
        }
    }
}
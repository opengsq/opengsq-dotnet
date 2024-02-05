using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    ServerName = ReadNullString(br, stripColor),
                    MapName = ReadNullString(br, stripColor),
                    GameType = ReadUnreal2String(br, stripColor),
                    NumPlayers = br.ReadInt32(),
                    MaxPlayers = br.ReadInt32(),
                    Ping = br.ReadInt32(),
                    Flags = br.ReadInt32(),
                    Skill = ReadUnreal2String(br, stripColor)
                };
            }
        }

        /// <summary>
        /// Gets the rules of the server.
        /// </summary>
        /// <param name="stripColor">A boolean value indicating whether to strip color codes. Default is true.</param>
        /// <returns>The rules of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Rules> GetRules(bool stripColor = true)
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, RULES });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, RULES);

                var rules = new Rules();

                while (!br.IsEnd())
                {
                    string key = ReadUnreal2String(br, stripColor);
                    string val = ReadUnreal2String(br, stripColor);

                    if (key.ToLower() == "mutator")
                    {
                        rules.Mutators.Add(val);
                    }
                    else
                    {
                        rules[key] = val;
                    }
                }

                return rules;
            }
        }

        /// <summary>
        /// Gets the players of the server.
        /// </summary>
        /// <param name="stripColor">A boolean value indicating whether to strip color codes. Default is true.</param>
        /// <returns>A list of players of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<List<Player>> GetPlayers(bool stripColor = true)
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
                        Name = ReadUnreal2String(br, stripColor),
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
        /// Reads a string from a binary reader.
        /// </summary>
        /// <param name="br">The binary reader to read the string from.</param>
        /// <param name="stripColor">Optional parameter. If set to true, color information will be stripped from the string. Default is false.</param>
        /// <returns>
        /// The string read from the binary reader. If 'stripColor' is true, the returned string will have color information stripped.
        /// </returns>
        protected string ReadUnreal2String(BinaryReader br, bool stripColor)
        {
            int length = br.ReadByte();

            string result;
            if (length >= 128)
            {
                length = (length & 0x7f) * 2;
                var bytes = br.ReadBytes(length).Where(b => b != 0xff && b != 0x00).ToArray();
                result = Encoding.Unicode.GetString(bytes);
            }
            else
            {
                var bytes = br.ReadBytes(length).Where(b => b != 0xff && b != 0x00).ToArray();
                result = Encoding.UTF8.GetString(bytes);
            }

            result = stripColor ? StripColor(result) : result;

            return result.Trim();
        }

        /// <summary>
        /// Reads a string from a binary stream, optionally stripping color information.
        /// </summary>
        /// <param name="br">The BinaryReader to read the string from.</param>
        /// <param name="stripColor">Optional parameter. If true, color information is stripped from the string.</param>
        /// <returns>The string read from the binary stream, with color information stripped if stripColor is true.</returns>
        protected string ReadNullString(BinaryReader br, bool stripColor = false)
        {
            br.ReadByte(); // Skip the length

            string result = br.ReadStringEx();

            result = stripColor ? StripColor(result) : result;

            return result;
        }
    }
}
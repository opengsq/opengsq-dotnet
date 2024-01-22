using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;
using OpenGSQ.Responses.ASE;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// All-Seeing Eye Protocol
    /// </summary>
    public class ASE : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "All-Seeing Eye Protocol";

        private readonly byte[] requestHeader = Encoding.ASCII.GetBytes("s");
        private readonly byte[] responseHeader = Encoding.ASCII.GetBytes("EYE1");

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
        /// Asynchronously gets the status of the server.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a Status object with the status information.
        /// </returns>
        /// <exception cref="InvalidPacketException">
        /// Thrown when the received packet header does not match the expected header.
        /// </exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus()
        {
            byte[] response = await UdpClient.CommunicateAsync(this, requestHeader);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                byte[] header = br.ReadBytes(4);
                InvalidPacketException.ThrowIfNotEqual(header, responseHeader);

                return new Status
                {
                    GameName = br.ReadPascalString(),
                    GamePort = int.Parse(br.ReadPascalString()),
                    Hostname = br.ReadPascalString(),
                    GameType = br.ReadPascalString(),
                    Map = br.ReadPascalString(),
                    Version = br.ReadPascalString(),
                    Password = br.ReadPascalString() != "0",
                    NumPlayers = int.Parse(br.ReadPascalString()),
                    MaxPlayers = int.Parse(br.ReadPascalString()),
                    Rules = ParseRules(br),
                    Players = ParsePlayers(br),
                };
            }
        }

        private Dictionary<string, string> ParseRules(BinaryReader br)
        {
            var rules = new Dictionary<string, string>();

            while (!br.IsEnd())
            {
                string key = br.ReadPascalString();

                if (string.IsNullOrEmpty(key))
                {
                    break;
                }

                rules[key] = br.ReadPascalString();
            }

            return rules;
        }

        private List<Player> ParsePlayers(BinaryReader br)
        {
            var players = new List<Player>();

            while (!br.IsEnd())
            {
                players.Add(ParsePlayer(br));
            }

            return players;
        }

        private Player ParsePlayer(BinaryReader br)
        {
            byte flags = br.ReadByte();
            var player = new Player();

            if ((flags & 1) == 1)
            {
                player.Name = br.ReadPascalString();
            }

            if ((flags & 2) == 2)
            {
                player.Team = br.ReadPascalString();
            }

            if ((flags & 4) == 4)
            {
                player.Skin = br.ReadPascalString();
            }

            if ((flags & 8) == 8)
            {
                player.Score = int.TryParse(br.ReadPascalString(), out int result) ? result : 0;
            }

            if ((flags & 16) == 16)
            {
                player.Ping = int.TryParse(br.ReadPascalString(), out int result) ? result : 0;
            }

            if ((flags & 32) == 32)
            {
                player.Time = int.TryParse(br.ReadPascalString(), out int result) ? result : 0;
            }

            return player;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using OpenGSQ.Responses.GameSpy2;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Gamespy Query Protocol version 2
    /// </summary>
    public class GameSpy2 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "GameSpy Protocol version 2";

        /// <summary>
        /// Initializes a new instance of the GameSpy2 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public GameSpy2(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {

        }

        /// <summary>
        /// Retrieves information about the server including Info, Players, and Teams.
        /// </summary>
        /// <param name="requestHeader">The type of information to request.</param>
        /// <returns>A Status object containing the requested information.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public async Task<Status> GetStatus(RequestHeader requestHeader = RequestHeader.Info | RequestHeader.Players | RequestHeader.Teams)
        {
            var request = new byte[] { 0xFE, 0xFD, 0x00, 0x04, 0x05, 0x06, 0x07 }.Concat(GetRequestBytes(requestHeader)).ToArray();
            var response = await UdpClient.CommunicateAsync(this, request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Skip first 5 bytes
                br.ReadBytes(5);

                var status = new Status();

                // Save Response Info
                if (requestHeader.HasFlag(RequestHeader.Info))
                {
                    status.Info = GetInfo(br);
                }

                // Save Response Players
                if (requestHeader.HasFlag(RequestHeader.Players))
                {
                    status.Players = GetPlayers(br);
                }

                // Save Response Teams
                if (requestHeader.HasFlag(RequestHeader.Teams))
                {
                    status.Teams = GetTeams(br);
                }

                return status;
            }
        }

        private byte[] GetRequestBytes(RequestHeader request)
        {
            return new byte[]
            {
                (byte)(request.HasFlag(RequestHeader.Info) ? 0xFF : 0x00),
                (byte)(request.HasFlag(RequestHeader.Players) ? 0xFF : 0x00),
                (byte)(request.HasFlag(RequestHeader.Teams) ? 0xFF : 0x00),
            };
        }

        private Dictionary<string, string> GetInfo(BinaryReader br)
        {
            var info = new Dictionary<string, string>();

            // Read all key values
            while (br.TryReadStringEx(out var key))
            {
                info[key] = br.ReadStringEx().Trim();
            }

            return info;
        }

        private List<Dictionary<string, string>> GetPlayers(BinaryReader br)
        {
            var players = new List<Dictionary<string, string>>();

            // Skip a byte
            br.ReadByte();

            // Get player count
            var playerCount = br.ReadByte();

            // Get all keys
            var keys = new List<string>();

            while (br.TryReadStringEx(out var key))
            {
                keys.Add(key.TrimEnd('_'));
            }

            // Set all keys and values
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Dictionary<string, string>());

                foreach (var key in keys)
                {
                    players[i][key] = br.ReadStringEx().Trim();
                }
            }

            return players;
        }

        private List<Dictionary<string, string>> GetTeams(BinaryReader br)
        {
            var teams = new List<Dictionary<string, string>>();

            // Skip a byte
            br.ReadByte();

            // Get team count
            var teamCount = br.ReadByte();

            // Get all keys
            var keys = new List<string>();

            while (br.TryReadStringEx(out var key))
            {
                keys.Add(key.TrimEnd('t').TrimEnd('_'));
            }

            // Set all keys and values
            for (int i = 0; i < teamCount; i++)
            {
                teams.Add(new Dictionary<string, string>());

                foreach (var key in keys)
                {
                    teams[i][key] = br.ReadStringEx().Trim();
                }
            }

            return teams;
        }

        /// <summary>
        /// Represents the types of requests that can be sent.
        /// </summary>
        [Flags]
        public enum RequestHeader : short
        {
            /// <summary>
            /// A request for information.
            /// </summary>
            Info = 1,

            /// <summary>
            /// A request for player data.
            /// </summary>
            Players = 2,

            /// <summary>
            /// A request for team data.
            /// </summary>
            Teams = 4,
        }
    }
}

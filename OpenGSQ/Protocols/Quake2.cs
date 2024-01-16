using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake2 Query Protocol
    /// </summary>
    public class Quake2 : Quake1
    {
        /// <inheritdoc/>
        public override string FullName => "Quake2 Query Protocol";

        /// <summary>
        /// Initializes a new instance of the Quake2 class.
        /// </summary>
        /// <param name="address">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Quake2(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {
            _RequestHeader = "status";
            _ResponseHeader = "print\n";
        }

        /// <summary>
        /// Gets the status of the server including information and players.
        /// </summary>
        /// <returns>A Status object containing the server information and players.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public new Status GetStatus()
        {
            using (var br = GetResponseBinaryReader())
            {
                return new Status
                {
                    Info = ParseInfo(br),
                    Players = ParsePlayers(br),
                };
            }
        }

        /// <summary>
        /// Parses the player information from the BinaryReader.
        /// </summary>
        /// <param name="br">The BinaryReader containing the player information.</param>
        /// <returns>A list of Player objects.</returns>
        protected new List<Player> ParsePlayers(BinaryReader br)
        {
            var players = new List<Player>();

            foreach (var matches in GetPlayerMatchCollections(br))
            {
                players.Add(new Player
                {
                    Frags = int.Parse(matches[0].Value),
                    Ping = int.Parse(matches[1].Value),
                    Name = matches.Count > 2 ? matches[2].Value.Trim('"') : null,
                    Address = matches.Count > 3 ? matches[3].Value.Trim('"') : null,
                });
            }

            return players;
        }

        /// <summary>
        /// Represents the status of the server.
        /// </summary>
        public new class Status
        {
            /// <summary>
            /// Gets or sets the server information.
            /// </summary>
            public Dictionary<string, string> Info { get; set; }

            /// <summary>
            /// Gets or sets the list of players.
            /// </summary>
            public List<Player> Players { get; set; }
        }

        /// <summary>
        /// Represents a player in the game.
        /// </summary>
        public new class Player
        {
            /// <summary>
            /// Gets or sets the player's frags.
            /// </summary>
            public int Frags { get; set; }

            /// <summary>
            /// Gets or sets the player's ping.
            /// </summary>
            public int Ping { get; set; }

            /// <summary>
            /// Gets or sets the player's name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the player's address.
            /// </summary>
            public string Address { get; set; }
        }
    }
}

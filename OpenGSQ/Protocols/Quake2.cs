﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenGSQ.Responses.Quake2;
using OpenGSQ.Exceptions;

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
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Quake2(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            RequestHeader = "status";
            ResponseHeader = "print\n";
        }

        /// <summary>
        /// Gets the status of the server including information and players.
        /// </summary>
        /// <returns>A Status object containing the server information and players.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public new async Task<Status> GetStatus()
        {
            using (var br = await GetResponseBinaryReader())
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
    }
}

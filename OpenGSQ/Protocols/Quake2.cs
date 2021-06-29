using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace OpenGSQ.Protocols
{
    public class Quake2 : Quake1
    {
        /// <summary>
        /// Quake2 Query Protocol
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public Quake2(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {
            _RequestHeader = "status";
            _ResponseHeader = "print\n";
        }

        /// <summary>
        /// This returns server information and players.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
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

        public new class Status
        {
            public Dictionary<string, string> Info { get; set; }

            public List<Player> Players { get; set; }
        }

        public new class Player
        {
            public int Frags { get; set; }

            public int Ping { get; set; }

            public string Name { get; set; }

            public string Address { get; set; }
        }
    }
}

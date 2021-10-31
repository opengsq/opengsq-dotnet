using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake1 Query Protocol
    /// </summary>
    public class Quake1 : ProtocolBase
    {
#pragma warning disable 1591
        protected byte _Delimiter1 = Encoding.ASCII.GetBytes("\\")[0];
        protected byte _Delimiter2 = Encoding.ASCII.GetBytes("\n")[0];
        protected string _RequestHeader, _ResponseHeader;
#pragma warning restore 1591

        /// <summary>
        /// Quake1 Query Protocol
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public Quake1(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {
            _RequestHeader = "status";
            _ResponseHeader = "n";
        }

        /// <summary>
        /// This returns server information and players.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Status GetStatus()
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

#pragma warning disable 1591
        protected BinaryReader GetResponseBinaryReader()
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSend(udpClient, _RequestHeader);

                var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8);
                var header = br.ReadStringEx(_Delimiter1);

                if (header != _ResponseHeader)
                {
                    throw new Exception($"Packet header mismatch. Received: {header}. Expected: {_ResponseHeader}.");
                }

                return br;
            }
        }

        protected Dictionary<string, string> ParseInfo(BinaryReader br)
        {
            var info = new Dictionary<string, string>();

            // Read all key values until meet \n
            while (br.TryReadStringEx(out var key, _Delimiter1))
            {
                info[key] = br.ReadStringEx(new byte[] { _Delimiter1, _Delimiter2 });

                br.BaseStream.Position--;

                if (br.ReadByte() == _Delimiter2)
                {
                    break;
                }
            }

            return info;
        }

        protected List<Player> ParsePlayers(BinaryReader br)
        {
            var players = new List<Player>();

            foreach (var matches in GetPlayerMatchCollections(br))
            {
                players.Add(new Player
                {
                    Id = int.Parse(matches[0].Value),
                    Score = int.Parse(matches[1].Value),
                    Time = int.Parse(matches[2].Value),
                    Ping = int.Parse(matches[3].Value),
                    Name = matches[4].Value.Trim('"'),
                    Skin = matches[5].Value.Trim('"'),
                    Color1 = int.Parse(matches[6].Value),
                    Color2 = int.Parse(matches[7].Value),
                });
            }

            return players;
        }

        protected List<MatchCollection> GetPlayerMatchCollections(BinaryReader br)
        {
            var matchCollections = new List<MatchCollection>();

            // Regex to split with whitespace and double quote
            var regex = new Regex("\"(\\\"|[^\"])*?\"|[^\\s]+");

            // Read all players
            while (br.BaseStream.Position < br.BaseStream.Length && br.TryReadStringEx(out var playerInfo, _Delimiter2))
            {
                matchCollections.Add(regex.Matches(playerInfo));
            }

            return matchCollections;
        }

        protected byte[] ConnectAndSend(UdpClient udpClient, string request)
        {
            // Connect to remote host
            udpClient.Connect(_EndPoint);
            udpClient.Client.SendTimeout = _Timeout;
            udpClient.Client.ReceiveTimeout = _Timeout;

            var header = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            // Send Request
            var requestData = new byte[0].Concat(header).Concat(Encoding.ASCII.GetBytes(request)).Concat(new byte[] { 0x00 } ).ToArray();
            udpClient.Send(requestData, requestData.Length);

            // Server response
            var responseData = udpClient.Receive(ref _EndPoint);

            // Remove the last 0x00 if exists (Only if Quake1)
            if (responseData[responseData.Length - 1] == 0)
            {
                responseData = responseData.Take(responseData.Length - 1).ToArray();
            }

            // Add \n at the last of responseData if not exists
            if (responseData[responseData.Length - 1] != _Delimiter2)
            {
                responseData = responseData.Concat(new byte[] { _Delimiter2 }).ToArray();
            }

            // Remove the first four 0xFF
            return responseData.Skip(header.Length).ToArray();
        }

        public class Status
        {
            public Dictionary<string, string> Info { get; set; }

            public List<Player> Players { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }

            public int Score { get; set; }

            public int Time { get; set; }

            public int Ping { get; set; }

            public string Name { get; set; }

            public string Skin { get; set; }

            public int Color1 { get; set; }

            public int Color2 { get; set; }
        }
    }
}

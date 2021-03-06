using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Gamespy Query Protocol version 3
    /// </summary>
    public class GameSpy3 : ProtocolBase
    {
#pragma warning disable 1591
        protected new bool _Challenge;
#pragma warning restore 1591

        /// <summary>
        /// Gamespy Query Protocol version 3
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public GameSpy3(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {

        }

        /// <summary>
        /// Retrieves information about the server including, Info, Players, and Teams.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Status GetStatus()
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSendPackets(udpClient);

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    return new Status
                    {
                        // Save Status Info
                        Info = GetInfo(br),

                        // Save Status Players
                        Players = GetPlayers(br),

                        // Save Status Teams
                        Teams = GetTeams(br)
                    };
                }
            }
        }

        private byte[] ConnectAndSendPackets(UdpClient udpClient)
        {
            // Connect to remote host
            udpClient.Connect(_EndPoint);
            udpClient.Client.SendTimeout = _Timeout;
            udpClient.Client.ReceiveTimeout = _Timeout;

            // Packet 1: Initial request
            byte[] responseData, challenge = new byte[] { }, requestData = new byte[] { 0xFE, 0xFD, 0x09, 0x04, 0x05, 0x06, 0x07 };

            if (_Challenge)
            {
                udpClient.Send(requestData, requestData.Length);

                // Packet 2: First response
                responseData = udpClient.Receive(ref _EndPoint);

                // Get challenge
                if (int.TryParse(Encoding.ASCII.GetString(responseData.Skip(5).ToArray()).Trim(), out int result) && result != 0)
                {
                    challenge = BitConverter.GetBytes(result);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(challenge);
                    }
                }
            }

            // Packet 3: Second request
            requestData[2] = 0x00;
            requestData = requestData.Concat(challenge).Concat(new byte[] { 0xFF, 0xFF, 0xFF, 0x01 }).ToArray();
            udpClient.Send(requestData, requestData.Length);

            // Packet 4: Server response
            responseData = Receive(udpClient);

            return responseData;
        }

        private byte[] Receive(UdpClient udpClient)
        {
            int totalPackets = -1;
            var payloads = new SortedDictionary<int, byte[]>();

            do
            {
                var responseData = udpClient.Receive(ref _EndPoint);

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    var header = br.ReadByte();

                    if (header != 0)
                    {
                        throw new Exception($"Packet header mismatch. Received: {header}. Expected: 0.");
                    }

                    // Skip the timestamp and splitnum
                    br.ReadBytes(13);

                    // The 'numPackets' byte
                    var numPackets = br.ReadByte();

                    // The low 7 bits are the packet index (starting at zero)
                    var number = numPackets & 0x7F;

                    // The high bit is whether or not this is the last packet
                    var isLastPacket = numPackets >> 7 == 1;

                    // Save totalPackets as packet number + 1
                    if (isLastPacket)
                    {
                        totalPackets = number + 1;
                    }

                    // The object id. Example: \x01
                    var objectId = br.ReadByte();

                    // The object header
                    byte[] objectHeader = new byte[] { };

                    if (objectId >= 1)
                    {
                        // The object name. Example: "player_"
                        string objectName = br.ReadStringEx();

                        // The object items appear count
                        int count = br.ReadByte();

                        // If the object item doesn't appear before, set the header back
                        if (count == 0)
                        {
                            // Set the header. Example: \x00\x01player_\x00\x00
                            objectHeader = new byte[] { 0x00, objectId }.Concat(Encoding.UTF8.GetBytes(objectName)).Concat(new byte[] { 0x00, 0x00 }).ToArray();
                        }
                    }

                    // Save the payload
                    byte[] payload = objectHeader.Concat(responseData.Skip((int)br.BaseStream.Position)).ToArray();

                    payloads.Add(number, TrimPayload(payload));
                }
            } while (totalPackets == -1 || payloads.Count < totalPackets);

            // Combine the payloads
            var combinedPayload = payloads.Values.Aggregate((a, b) => a.Concat(b).ToArray());

            return combinedPayload;
        }

        /// <summary>
        /// Remove the last trash string on the payload 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private byte[] TrimPayload(byte[] payload)
        {
            int i = payload.Length;

            while (payload[--i] != 0);

            return payload.Take(i).ToArray();
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

            // Return if BaseStream is end
            if (br.BaseStream.Position == br.BaseStream.Length)
            {
                return players;
            }

            // Skip \x01player_\x00\x00
            br.ReadByte();
            string key = br.ReadStringEx().TrimEnd('_');
            br.ReadByte();

            // Team index
            int i = 0;

            // Loop all values and save
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                if (br.TryReadStringEx(out var value))
                {
                    // Add a Dictionary object if not exists
                    if (players.Count < i + 1)
                    {
                        players.Add(new Dictionary<string, string>());
                    }

                    // Save the value
                    players[i++][key] = value.Trim();
                }
                else
                {
                    // Return if no player
                    if (br.BaseStream.Position == br.BaseStream.Length)
                    {
                        break;
                    }

                    // Set new key
                    if (br.TryReadStringEx(out key))
                    {
                        // Remove the trailing "_"
                        key = key.TrimEnd('_');
                    }
                    else
                    {
                        break;
                    }

                    // Reset the team index
                    i = br.ReadByte();
                }
            }

            return players;
        }

        private List<Dictionary<string, string>> GetTeams(BinaryReader br)
        {
            var teams = new List<Dictionary<string, string>>();

            // Return if BaseStream is end
            if (br.BaseStream.Position == br.BaseStream.Length)
            {
                return teams;
            }

            // Skip \x00\x02team_t\x00\x00
            br.ReadBytes(2);
            string key = br.ReadStringEx().TrimEnd('t').TrimEnd('_');
            br.ReadByte();

            // Player index
            int i = 0;

            // Loop all values and save
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                if (br.TryReadStringEx(out var value))
                {
                    // Add a Dictionary object if not exists
                    if (teams.Count < i + 1)
                    {
                        teams.Add(new Dictionary<string, string>());
                    }

                    // Save the value
                    teams[i++][key] = value.Trim();
                }
                else
                {
                    // Return if no team
                    if (br.BaseStream.Position == br.BaseStream.Length)
                    {
                        break;
                    }

                    // Set new key
                    if (br.TryReadStringEx(out key))
                    {
                        // Remove the trailing "_t"
                        key = key.TrimEnd('t').TrimEnd('_');
                    }
                    else
                    {
                        break;
                    }

                    // Reset the team index
                    i = br.ReadByte();
                }
            }

            return teams;
        }

        /// <summary>
        /// Status object
        /// </summary>
        public class Status
        {
            /// <summary>
            /// Status Info
            /// </summary>
            public Dictionary<string, string> Info { get; set; }

            /// <summary>
            /// Status Players
            /// </summary>
            public List<Dictionary<string, string>> Players { get; set; }

            /// <summary>
            /// Status Teams
            /// </summary>
            public List<Dictionary<string, string>> Teams { get; set; }
        }
    }
}

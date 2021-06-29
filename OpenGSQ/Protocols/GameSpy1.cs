using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OpenGSQ.Protocols
{
    public class GameSpy1 : ProtocolBase
    {
        private static readonly byte _delimiter = Encoding.ASCII.GetBytes("\\")[0];

        /// <summary>
        /// Gamespy Query Protocol version 1
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public GameSpy1(string address, int port, int timeout = 5000) : base(address, port, timeout)
        {

        }

        /// <summary>
        /// This returns basic server information, mainly for recognition.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public Dictionary<string, string> GetBasic()
        {
            return SendAndParseKeyValue("\\basic\\");
        }

        /// <summary>
        /// Information about the current game running on the server.
        /// <para>If the server uses XServerQuery, he sends you the new information, otherwise he'll give you back the old information.</para>
        /// </summary> 
        /// <param name="XServerQuery"></param>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public Dictionary<string, string> GetInfo(bool XServerQuery = true)
        {
            return SendAndParseKeyValue("\\info\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// Setting for the current game, return sets of rules depends on the running game type.
        /// <para>If the server uses XServerQuery, he sends you the new information, otherwise he'll give you back the old information.</para>
        /// </summary>
        /// <param name="XServerQuery"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Dictionary<string, string> GetRules(bool XServerQuery = true)
        {
            return SendAndParseKeyValue("\\rules\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// Returns information about each player on the server.
        /// <para>If the server uses XServerQuery, he sends you the new information, otherwise he'll give you back the old information.</para>
        /// </summary>
        /// <param name="XServerQuery"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public List<Dictionary<string, string>> GetPlayers(bool XServerQuery = true)
        {
            return SendAndParseObject("\\players\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// XServerQuery: \info\xserverquery\rules\xserverquery\players\xserverquery<br />
        /// Old response: \basic\\info\\rules\\players\
        /// <para>If the server uses XServerQuery, he sends you the new information, otherwise he'll give you back the old information.</para>
        /// </summary>
        /// <param name="XServerQuery"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public Status GetStatus(bool XServerQuery = true)
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSend(udpClient, "\\status\\" + (XServerQuery ? "xserverquery" : string.Empty));

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    var status = new Status
                    {
                        KeyValues = new Dictionary<string, string>()
                    };

                    long position = 0;

                    // Read key until "player_#" or "Player_#"
                    while (br.BaseStream.Position < br.BaseStream.Length 
                        && br.TryReadStringEx(out var key, _delimiter) 
                        && !key.ToLower().StartsWith("player_"))
                    {
                        // Save key and value
                        status.KeyValues[key] = br.ReadStringEx(_delimiter);

                        // Save the position after read the value
                        position = br.BaseStream.Position;
                    }

                    // Reset the position
                    br.BaseStream.Position = position;

                    // Save players
                    status.Players = ParseObject(br);

                    if (status.IsXServerQuery)
                    {
                        // Save teams if it is XServerQuery response
                        status.Teams = new List<Dictionary<string, string>>();

                        var teams = status.KeyValues.Where(x => x.Key.Contains('_'));

                        foreach (KeyValuePair<string, string> keyValue in teams)
                        {
                            // Split key and index
                            string[] subs = keyValue.Key.Split(new char[] { '_' }, 2);
                            (string key, int index) = (subs[0], int.Parse(subs[1]));

                            // Create a new team if not exists
                            if (status.Teams.Count <= index)
                            {
                                status.Teams.Add(new Dictionary<string, string>());
                            }

                            // Set the value
                            status.Teams[index][key] = keyValue.Value;

                            // Remove the key
                            status.KeyValues.Remove(keyValue.Key);
                        }
                    }

                    return status;
                }
            }
        }

        /// <summary>
        /// Returns information about each team on the server.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public List<Dictionary<string, string>> GetTeams()
        {
            return SendAndParseObject("\\teams\\");
        }

        /// <summary>
        /// This command requires a argument, the argument will be returned.
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public Dictionary<string, string> GetEcho(string text = "this is a test")
        {
            return SendAndParseKeyValue("\\echo\\" + text);
        }

        private Dictionary<string, string> ParseKeyValue(BinaryReader br)
        {
            var kv = new Dictionary<string, string>();

            // Read all key values
            while (br.BaseStream.Position < br.BaseStream.Length && br.TryReadStringEx(out var key, _delimiter))
            {
                kv[key] = br.ReadStringEx(_delimiter);
            }

            return kv;
        }

        private List<Dictionary<string, string>> ParseObject(BinaryReader br)
        {
            var items = new List<Dictionary<string, string>>();

            while (br.BaseStream.Position < br.BaseStream.Length && br.TryReadStringEx(out var outString, _delimiter))
            {
                // Split key and index
                string[] subs = outString.Split(new char[] { '_' }, 2);
                (string key, int index) = (subs[0], int.Parse(subs[1]));

                // Create a new item if not exists
                while (items.Count <= index)
                {
                    items.Add(new Dictionary<string, string>());
                }

                // Set the value
                items[index][key] = br.ReadStringEx(_delimiter);
            }

            return items;
        }

        private Dictionary<string, string> SendAndParseKeyValue(string request)
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSend(udpClient, request);

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    return ParseKeyValue(br);
                }
            }
        }

        private List<Dictionary<string, string>> SendAndParseObject(string request)
        {
            using (var udpClient = new UdpClient())
            {
                var responseData = ConnectAndSend(udpClient, request);

                using (var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8))
                {
                    return ParseObject(br);
                }
            }
        }

        private byte[] ConnectAndSend(UdpClient udpClient, string request)
        {
            // Connect to remote host
            udpClient.Connect(_EndPoint);
            udpClient.Client.SendTimeout = _Timeout;
            udpClient.Client.ReceiveTimeout = _Timeout;

            // Send Request
            var requestData = Encoding.ASCII.GetBytes(request);
            udpClient.Send(requestData, requestData.Length);

            // Server response
            var responseData = Receive(udpClient);

            // Remove
            return responseData;
        }

        private byte[] Receive(UdpClient udpClient)
        {
            int totalPackets = -1, packetId;
            var payloads = new SortedDictionary<int, byte[]>();

            do
            {
                var responseData = udpClient.Receive(ref _EndPoint);

                // Try read "queryid" value, if it is the last packet, it cannot read the "queryid" value directly
                if (!ReadStringReverse(responseData, responseData.Length, out var endIndex, out var queryId))
                {
                    // Read "final" string
                    ReadStringReverse(responseData, endIndex, out endIndex, out _);

                    // Read "queryid" value
                    ReadStringReverse(responseData, endIndex, out endIndex, out queryId);

                    // Save total packet
                    totalPackets = int.Parse(queryId.Split('.')[1]);
                }

                // Get packet id
                packetId = int.Parse(queryId.Split('.')[1]);

                // Read "queryid" string
                ReadStringReverse(responseData, endIndex, out endIndex, out _);

                // Save the payload
                byte[] payload = responseData.Take(endIndex).Skip(1).Concat(new byte[] { _delimiter }).ToArray();

                payloads.Add(packetId, payload);
            } while (totalPackets == -1 || payloads.Count < totalPackets);

            // Combine the payloads
            var combinedPayload = payloads.Values.Aggregate((a, b) => a.Concat(b).ToArray());

            return combinedPayload;
        }

        private bool ReadStringReverse(byte[] data, int startIndex, out int endIndex, out string outString)
        {
            var bytes = new List<byte>();

            while (data[--startIndex] != _delimiter)
            {
                bytes.Add(data[startIndex]);
            }

            bytes.Reverse();

            endIndex = startIndex;
            outString = Encoding.UTF8.GetString(bytes.ToArray());

            return !string.IsNullOrEmpty(outString);
        }

        /// <summary>
        /// Server Status
        /// </summary>
        public class Status
        {
            /// <summary>
            /// Indicates whether the response is XServerQuery or old response.
            /// </summary>
            public bool IsXServerQuery { get => KeyValues?.ContainsKey("XServerQuery") ?? false; }

            /// <summary>
            /// Server's KeyValues
            /// <para>If <see cref="IsXServerQuery"/> is <see langword="true"/>, then it includes \info\xserverquery\rules\xserverquery, else \basic\\info\\rules\\</para>
            /// </summary>
            public Dictionary<string, string> KeyValues { get; set; }

            /// <summary>
            /// Server's Players
            /// </summary>
            public List<Dictionary<string, string>> Players { get; set; }

            /// <summary>
            /// Server's Teams (Only when <see cref="IsXServerQuery"/> is <see langword="true"/>)
            /// </summary>
            public List<Dictionary<string, string>> Teams { get; set; }
        }
    }
}

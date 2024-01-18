using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Responses.GameSpy1;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Gamespy Query Protocol version 1
    /// </summary>
    public class GameSpy1 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "GameSpy Protocol version 1";

        private static readonly byte _delimiter = Encoding.ASCII.GetBytes("\\")[0];

        /// <summary>
        /// Initializes a new instance of the GameSpy1 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public GameSpy1(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {

        }

        /// <summary>
        /// Gets basic server information, mainly for recognition.
        /// </summary>
        /// <returns>A dictionary containing the basic server information.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<Dictionary<string, string>> GetBasic()
        {
            return SendAndParseKeyValue("\\basic\\");
        }

        /// <summary>
        /// Gets information about the current game running on the server.
        /// </summary>
        /// <param name="XServerQuery">A boolean indicating whether to use XServerQuery.</param>
        /// <returns>A dictionary containing the game information.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<Dictionary<string, string>> GetInfo(bool XServerQuery = true)
        {
            return SendAndParseKeyValue("\\info\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// Gets the settings for the current game, returns sets of rules depends on the running game type.
        /// </summary>
        /// <param name="XServerQuery">A boolean indicating whether to use XServerQuery.</param>
        /// <returns>A dictionary containing the game rules.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<Dictionary<string, string>> GetRules(bool XServerQuery = true)
        {
            return SendAndParseKeyValue("\\rules\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// Returns information about each player on the server.
        /// </summary>
        /// <param name="XServerQuery">A boolean indicating whether to use XServerQuery.</param>
        /// <returns>A list of dictionaries containing the player information.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<List<Dictionary<string, string>>> GetPlayers(bool XServerQuery = true)
        {
            return SendAndParseObject("\\players\\" + (XServerQuery ? "xserverquery" : string.Empty));
        }

        /// <summary>
        /// Gets the status of the server. If the server uses XServerQuery, it sends the new information, otherwise it gives back the old information.
        /// </summary>
        /// <param name="XServerQuery">A boolean indicating whether to use XServerQuery.</param>
        /// <returns>A Status object containing the server information, players, and teams.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public async Task<Status> GetStatus(bool XServerQuery = true)
        {
            byte[] response = await ConnectAndSend("\\status\\" + (XServerQuery ? "xserverquery" : string.Empty));

            using (var br = new BinaryReader(new MemoryStream(response)))
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

        /// <summary>
        /// Returns information about each team on the server.
        /// </summary>
        /// <returns>A list of dictionaries containing the team information.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<List<Dictionary<string, string>>> GetTeams()
        {
            return SendAndParseObject("\\teams\\");
        }

        /// <summary>
        /// Sends an echo command to the server. The server will return the argument.
        /// </summary>
        /// <param name="text">The text to send with the echo command.</param>
        /// <returns>A dictionary containing the server's response.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public Task<Dictionary<string, string>> GetEcho(string text = "this is a test")
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

        private async Task<Dictionary<string, string>> SendAndParseKeyValue(string request)
        {
            byte[] response = await ConnectAndSend(request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                return ParseKeyValue(br);
            }
        }

        private async Task<List<Dictionary<string, string>>> SendAndParseObject(string request)
        {
            byte[] response = await ConnectAndSend(request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                return ParseObject(br);
            }
        }

        private async Task<byte[]> ConnectAndSend(string request)
        {
            using (var udpClient = new System.Net.Sockets.UdpClient())
            {
                udpClient.Client.SendTimeout = Timeout;
                udpClient.Client.ReceiveTimeout = Timeout;

                // Connect to remote host
                udpClient.Connect(Host, Port);

                // Send Request
                byte[] datagram = Encoding.ASCII.GetBytes(request);
                await udpClient.SendAsync(datagram, datagram.Length);

                // Server response
                byte[] response = await Receive(udpClient);

                // Remove
                return response;
            }
        }

        private async Task<byte[]> Receive(System.Net.Sockets.UdpClient udpClient)
        {
            int totalPackets = -1, packetId;
            var payloads = new SortedDictionary<int, byte[]>();

            do
            {
                byte[] response = await udpClient.ReceiveAsyncWithTimeout();

                // Try read "queryid" value, if it is the last packet, it cannot read the "queryid" value directly
                if (!ReadStringReverse(response, response.Length, out var endIndex, out var queryId))
                {
                    // Read "final" string
                    ReadStringReverse(response, endIndex, out endIndex, out _);

                    // Read "queryid" value
                    ReadStringReverse(response, endIndex, out endIndex, out queryId);

                    // Save total packet
                    totalPackets = int.Parse(queryId.Split('.')[1]);
                }

                // Get packet id
                packetId = int.Parse(queryId.Split('.')[1]);

                // Read "queryid" string
                ReadStringReverse(response, endIndex, out endIndex, out _);

                // Save the payload
                byte[] payload = response.Take(endIndex).Skip(1).Concat(new byte[] { _delimiter }).ToArray();

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
    }
}

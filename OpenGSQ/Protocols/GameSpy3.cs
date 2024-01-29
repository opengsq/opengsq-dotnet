using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Responses.GameSpy2;
using OpenGSQ.Exceptions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Gamespy Query Protocol version 3
    /// </summary>
    public class GameSpy3 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "GameSpy Protocol version 3";

        /// <summary>
        /// A boolean indicating whether to use the challenge method.
        /// </summary>
        protected bool ChallengeRequired;

        /// <summary>
        /// Initializes a new instance of the GameSpy3 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public GameSpy3(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {

        }

        /// <summary>
        /// Retrieves information about the server including Info, Players, and Teams.
        /// </summary>
        /// <returns>A Status object containing the server information, players, and teams.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus()
        {
            byte[] response = await ConnectAndSendPackets();

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                return new Status
                {
                    // Save Status Info
                    Info = GetInfo(br),

                    // Save Status Players
                    Players = GetDictionaries(br, "player"),

                    // Save Status Teams
                    Teams = GetDictionaries(br, "team")
                };
            }
        }

        private async Task<byte[]> ConnectAndSendPackets()
        {
            using (var udpClient = new System.Net.Sockets.UdpClient())
            {
                // Connect to remote host
                udpClient.Connect(Host, Port, Timeout);

                // Packet 1: Initial request
                byte[] responseData, challenge = new byte[] { }, requestData = new byte[] { 0xFE, 0xFD, 0x09, 0x04, 0x05, 0x06, 0x07 };

                if (ChallengeRequired)
                {
                    await udpClient.SendAsync(requestData, requestData.Length);

                    // Packet 2: First response
                    responseData = await udpClient.ReceiveAsyncWithTimeout();

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
                responseData = await Receive(udpClient);

                return responseData;
            }
        }

        private async Task<byte[]> Receive(System.Net.Sockets.UdpClient udpClient)
        {
            int totalPackets = -1;
            var payloads = new SortedDictionary<int, byte[]>();

            do
            {
                byte[] response = await udpClient.ReceiveAsyncWithTimeout();

                using (var br = new BinaryReader(new MemoryStream(response)))
                {
                    var header = br.ReadByte();
                    InvalidPacketException.ThrowIfNotEqual(header, 0);

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
                    byte[] payload = objectHeader.Concat(response.Skip((int)br.BaseStream.Position)).ToArray();

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

            while (payload[--i] != 0) ;

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

        private List<Dictionary<string, string>> GetDictionaries(BinaryReader br, string objectType)
        {
            var kvs = new List<Dictionary<string, string>>();

            // Return if BaseStream is end
            if (br.IsEnd())
            {
                return kvs;
            }

            // Skip a byte
            br.ReadByte();

            // Player/Team index
            int i = 0;

            while (!br.IsEnd())
            {
                if (br.TryReadStringEx(out string key))
                {
                    // Skip \x00
                    br.ReadByte();

                    // Remove the trailing "_t"
                    key = key.TrimEnd('t').TrimEnd('_');

                    // Change the key to name
                    if (key == objectType)
                    {
                        key = "name";
                    }

                    while (!br.IsEnd() && br.TryReadStringEx(out string value))
                    {
                        // Add a Dictionary object if not exists
                        if (kvs.Count < i + 1)
                        {
                            kvs.Add(new Dictionary<string, string>());
                        }

                        kvs[i++][key] = value.Trim();
                    }

                    i = 0;
                }
                else
                {
                    break;
                }
            }

            return kvs;
        }
    }
}

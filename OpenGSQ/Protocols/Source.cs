using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Checksum;
using OpenGSQ.Responses.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = OpenGSQ.Responses.Source.Environment;
using OpenGSQ.Exceptions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Source Engine Query Protocol
    /// </summary>
    public class Source : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Source Engine Protocol";

        /// <summary>
        /// The byte array representing the A2S_INFO request.
        /// </summary>
        protected byte[] A2S_INFO = new byte[] { 0x54 };

        /// <summary>
        /// The byte array representing the A2S_PLAYER request.
        /// </summary>
        protected byte[] A2S_PLAYER = new byte[] { 0x55 };

        /// <summary>
        /// The byte array representing the A2S_RULES request.
        /// </summary>
        protected byte[] A2S_RULES = new byte[] { 0x56 };

        /// <summary>
        /// Initializes a new instance of the Source class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to. Default is 27015.</param>
        /// <param name="timeout">The connection timeout in milliseconds. Default is 5 seconds.</param>
        public Source(string host, int port = 27015, int timeout = 5000) : base(host, port, timeout)
        {

        }

        /// <summary>
        /// Retrieves information about the server including, but not limited to: its name, the map currently being played, and the number of players.<br />
        /// See: <see href="https://developer.valvesoftware.com/wiki/Server_queries#A2S_INFO">https://developer.valvesoftware.com/wiki/Server_queries#A2S_INFO</see>
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the server information.
        /// </returns>
        /// <exception cref="InvalidPacketException">Thrown when the received packet type is not S2A_INFO_SRC or S2A_INFO_DETAILED.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<PartialInfo> GetInfo()
        {
            byte[] response = await ConnectAndSendChallenge(A2S_INFO);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadByte();

                if (header != (byte)QueryResponse.S2A_INFO_SRC && header != (byte)QueryResponse.S2A_INFO_DETAILED)
                {
                    throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {QueryResponse.S2A_INFO_SRC} or {QueryResponse.S2A_INFO_DETAILED}.");
                }

                if (header == (byte)QueryResponse.S2A_INFO_SRC)
                {
                    var source = new SourceInfo
                    {
                        Protocol = br.ReadByte(),
                        Name = br.ReadStringEx(),
                        Map = br.ReadStringEx(),
                        Folder = br.ReadStringEx(),
                        Game = br.ReadStringEx(),
                        ID = br.ReadInt16(),
                        Players = br.ReadByte(),
                        MaxPlayers = br.ReadByte(),
                        Bots = br.ReadByte(),
                        ServerType = (ServerType)br.ReadByte(),
                        Environment = GetEnvironment(br.ReadByte()),
                        Visibility = (Visibility)br.ReadByte(),
                        VAC = (VAC)br.ReadByte()
                    };

                    if (source.ID == 2400)
                    {
                        source.Mode = br.ReadByte();
                        source.Witnesses = br.ReadByte();
                        source.Duration = br.ReadByte();
                    }

                    source.Version = br.ReadStringEx();

                    if (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        source.EDF = (ExtraDataFlag)br.ReadByte();

                        var edf = (ExtraDataFlag)source.EDF;

                        if (edf.HasFlag(ExtraDataFlag.Port))
                        {
                            source.Port = br.ReadInt16();
                        }

                        if (edf.HasFlag(ExtraDataFlag.SteamID))
                        {
                            source.SteamID = br.ReadUInt64();
                        }

                        if (edf.HasFlag(ExtraDataFlag.Spectator))
                        {
                            source.SpectatorPort = br.ReadInt16();
                            source.SpectatorName = br.ReadStringEx();
                        }

                        if (edf.HasFlag(ExtraDataFlag.Keywords))
                        {
                            source.Keywords = br.ReadStringEx();
                        }

                        if (edf.HasFlag(ExtraDataFlag.SteamID))
                        {
                            source.GameID = br.ReadUInt64();
                        }
                    }

                    return source;
                }
                else
                {
                    var goldSource = new GoldSourceInfo
                    {
                        Address = br.ReadStringEx(),
                        Name = br.ReadStringEx(),
                        Map = br.ReadStringEx(),
                        Folder = br.ReadStringEx(),
                        Game = br.ReadStringEx(),
                        Players = br.ReadByte(),
                        MaxPlayers = br.ReadByte(),
                        Protocol = br.ReadByte(),
                        ServerType = (ServerType)char.ToLower(Convert.ToChar(br.ReadByte())),
                        Environment = (Environment)char.ToLower(Convert.ToChar(br.ReadByte())),
                        Visibility = (Visibility)br.ReadByte(),
                        Mod = br.ReadByte()
                    };

                    if (goldSource.Mod == 1)
                    {
                        goldSource.Link = br.ReadStringEx();
                        goldSource.DownloadLink = br.ReadStringEx();
                        br.ReadByte();
                        goldSource.Version = br.ReadInt32();
                        goldSource.Size = br.ReadInt32();
                        goldSource.Type = br.ReadByte();
                        goldSource.DLL = br.ReadByte();
                    }

                    goldSource.VAC = (VAC)br.ReadByte();
                    goldSource.Bots = br.ReadByte();

                    return goldSource;
                }
            }
        }

        /// <summary>
        /// This query retrieves information about the players currently on the server.<br />
        /// See: <see href="https://developer.valvesoftware.com/wiki/Server_queries#A2S_PLAYER">https://developer.valvesoftware.com/wiki/Server_queries#A2S_PLAYER</see>
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of players.
        /// </returns>
        /// <exception cref="InvalidPacketException">Thrown when the received packet type is not S2A_PLAYER.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<List<Player>> GetPlayers()
        {
            byte[] response = await ConnectAndSendChallenge(A2S_PLAYER);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, (byte)QueryResponse.S2A_PLAYER);

                var playerCount = br.ReadByte();
                var players = new List<Player>();

                // Save the players
                for (int i = 0; i < playerCount; i++)
                {
                    br.ReadByte();

                    players.Add(new Player
                    {
                        Name = br.ReadStringEx(),
                        Score = br.ReadInt32(),
                        Duration = br.ReadSingle(),
                    });
                }

                if (!br.IsEnd())
                {
                    for (int i = 0; i < playerCount; i++)
                    {
                        players[i].Deaths = br.ReadInt32();
                        players[i].Money = br.ReadInt32();
                    }
                }

                return players;
            }
        }

        /// <summary>
        /// Returns the server rules, or configuration variables in name/value pairs.<br />
        /// See: <see href="https://developer.valvesoftware.com/wiki/Server_queries#A2S_RULES">https://developer.valvesoftware.com/wiki/Server_queries#A2S_RULES</see>
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a dictionary of rules where the rule name is the key and the rule value is the value.
        /// </returns>
        /// <exception cref="InvalidPacketException">Thrown when the received packet type is not S2A_RULES.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Dictionary<string, string>> GetRules()
        {
            byte[] response = await ConnectAndSendChallenge(A2S_RULES);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, (byte)QueryResponse.S2A_RULES);

                var ruleCount = br.ReadUInt16();
                var rules = new Dictionary<string, string>();

                // Save the rules into dictionary
                for (int i = 0; i < ruleCount; i++)
                {
                    rules.Add(br.ReadStringEx(), br.ReadStringEx());
                }

                return rules;
            }
        }

        private async Task<byte[]> ConnectAndSendChallenge(byte[] header)
        {
            using (var udpClient = new System.Net.Sockets.UdpClient())
            {
                udpClient.Client.SendTimeout = Timeout;
                udpClient.Client.ReceiveTimeout = Timeout;

                // Connect to remote host
                udpClient.Connect(Host, Port);

                // Set up request base
                byte[] requestBase = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }.Concat(header).ToArray();

                if (header.Equals(A2S_INFO))
                {
                    requestBase = requestBase.Concat(Encoding.Default.GetBytes("Source Engine Query\0")).ToArray();
                }

                // Set up request data
                byte[] requestData = requestBase;

                if (!header.Equals(A2S_INFO))
                {
                    requestData = requestData.Concat(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }).ToArray();
                }

                // Send and receive
                await udpClient.SendAsync(requestData, requestData.Length);
                byte[] response = await Receive(udpClient);

                // The server may reply with a challenge
                if (response[0] == (byte)QueryResponse.S2C_CHALLENGE)
                {
                    byte[] challenge = response.Skip(1).ToArray();

                    // Send the challenge and receive
                    requestData = requestBase.Concat(challenge).ToArray();
                    await udpClient.SendAsync(requestData, requestData.Length);
                    response = await Receive(udpClient);
                }

                return response;
            }
        }

        private async Task<byte[]> Receive(System.Net.Sockets.UdpClient udpClient)
        {
            bool isCompressed;
            int totalPackets = -1, crc32Sum = 0;
            var payloads = new SortedDictionary<int, byte[]>();
            var packets = new List<byte[]>();

            do
            {
                byte[] response = await udpClient.ReceiveAsyncWithTimeout();
                packets.Add(response);

                using (var br = new BinaryReader(new MemoryStream(response)))
                {
                    var header = br.ReadInt32();

                    // Simple Response Format
                    if (header == -1)
                    {
                        // Return the payload
                        return response.Skip((int)br.BaseStream.Position).ToArray();
                    }

                    // Packet id
                    int id = br.ReadInt32();
                    isCompressed = id < 0;

                    // Check is GoldSource multi-packet response format
                    if (IsGoldSourceSplit(response, (int)br.BaseStream.Position))
                    {
                        // Return the payload
                        return await ParseGoldSourcePackets(udpClient, packets);
                    }

                    // The total number of packets
                    totalPackets = br.ReadByte();

                    // The number of the packet
                    int number = br.ReadByte();

                    // Packet size
                    br.ReadUInt16();

                    if (number == 0 && isCompressed)
                    {
                        // Decompressed size
                        br.ReadInt32();

                        // CRC32 sum
                        crc32Sum = br.ReadInt32();
                    }

                    payloads.Add(number, response.Skip((int)br.BaseStream.Position).ToArray());
                }
            } while (totalPackets == -1 || payloads.Count < totalPackets);

            // Combine the payloads
            var combinedPayload = payloads.Values.Aggregate((a, b) => a.Concat(b).ToArray());

            // Decompress the payload
            if (isCompressed)
            {
                using (var compressedData = new MemoryStream(combinedPayload))
                using (var uncompressedData = new MemoryStream())
                {
                    BZip2.Decompress(compressedData, uncompressedData, true);
                    combinedPayload = uncompressedData.ToArray();
                }

                // Check CRC32 sum
                var crc32 = new Crc32();
                crc32.Update(combinedPayload);

                if (crc32.Value != crc32Sum)
                {
                    throw new InvalidPacketException("CRC32 checksum mismatch of uncompressed packet data.");
                }
            }

            return combinedPayload.Skip(4).ToArray();
        }

        private bool IsGoldSourceSplit(byte[] responseData, int streamPosition)
        {
            var data = responseData.Skip(streamPosition).ToArray();

            // Upper 4 bits represent the number of the current packet (starting at 0)
            int number = data[0] >> 4;

            // Check is it Gold Source packet split format
            return number == 0 && data[1] == 0xFF && data[2] == 0xFF && data[3] == 0xFF && data[4] == 0xFF;
        }

        private async Task<byte[]> ParseGoldSourcePackets(System.Net.Sockets.UdpClient udpClient, List<byte[]> packets)
        {
            int totalPackets = -1;
            var payloads = new SortedDictionary<int, byte[]>();

            while (totalPackets == -1 || payloads.Count < totalPackets)
            {
                // Load the old received packets first, then receive the packets from udpClient
                byte[] response = payloads.Count < packets.Count ? packets[payloads.Count] : await udpClient.ReceiveAsyncWithTimeout();

                using (var br = new BinaryReader(new MemoryStream(response)))
                {
                    // Header
                    br.ReadInt32();

                    // Packet id
                    br.ReadInt32();

                    // The total number of packets
                    totalPackets = br.ReadByte();

                    // Upper 4 bits represent the number of the current packet (starting at 0)
                    int number = totalPackets >> 4;

                    // Bottom 4 bits represent the total number of packets (2 to 15)
                    totalPackets &= 0x0F;

                    payloads.Add(number, response.Skip((int)br.BaseStream.Position).ToArray());
                }
            }

            // Combine the payloads
            var combinedPayload = payloads.Values.Aggregate((a, b) => a.Concat(b).ToArray());

            return combinedPayload.Skip(4).ToArray();
        }

        private Environment GetEnvironment(byte environmentByte)
        {
            switch (environmentByte)
            {
                case (byte)Environment.Linux:
                    return Environment.Linux;
                case (byte)Environment.Windows:
                    return Environment.Windows;
                default:
                    return Environment.Mac;
            }
        }

        private enum QueryResponse : byte
        {
            S2C_CHALLENGE = 0x41,
            S2A_INFO_SRC = 0x49,
            S2A_INFO_DETAILED = 0x6D,
            S2A_PLAYER = 0x44,
            S2A_RULES = 0x45,
        }

        /// <summary>
        /// Source RCON Protocol
        /// </summary>
        public class RemoteConsole : ProtocolBase, IDisposable
        {
            private System.Net.Sockets.TcpClient _tcpClient;

            /// <inheritdoc/>
            public override string FullName => "Source RCON Protocol";

            /// <summary>
            /// Source RCON Protocol
            /// </summary>
            /// <param name="address"></param>
            /// <param name="port"></param>
            /// <param name="timeout"></param>
            public RemoteConsole(string address, int port = 27015, int timeout = 5000) : base(address, port, timeout)
            {

            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _tcpClient?.Close();
            }

            /// <summary>
            /// Authenticates the client with the server using the provided password.
            /// </summary>
            /// <param name="password">The password to be used for authentication.</param>
            /// <returns>A task that represents the asynchronous operation.</returns>
            /// <exception cref="ArgumentException">Thrown when the password is null or empty.</exception>
            /// <exception cref="InvalidPacketException">Thrown when the received packet type is not SERVERDATA_AUTH_RESPONSE.</exception>
            /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
            /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
            public async Task Authenticate(string password)
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Password cannot be null or empty. Please provide a valid password.");
                }

                // Connect
                _tcpClient = new System.Net.Sockets.TcpClient();
                await _tcpClient.ConnectAsync(Host, Port);
                _tcpClient.Client.SendTimeout = Timeout;
                _tcpClient.Client.ReceiveTimeout = Timeout;

                // Send password
                int id = new Random().Next(4096);
                await _tcpClient.SendAsync(new Packet(id, PacketType.SERVERDATA_AUTH, password).GetBytes());

                // Receive and parse as Packet
                var buffer = await _tcpClient.ReceiveAsync();
                var packet = new Packet(buffer);

                // Sometimes it will return a PacketType.SERVERDATA_RESPONSE_VALUE, so receive again
                if (packet.Type != PacketType.SERVERDATA_AUTH_RESPONSE)
                {
                    buffer = await _tcpClient.ReceiveAsync();
                    packet = new Packet(buffer);
                }

                // Throw exception if not PacketType.SERVERDATA_AUTH_RESPONSE
                if (packet.Type != PacketType.SERVERDATA_AUTH_RESPONSE)
                {
                    _tcpClient.Close();
                    InvalidPacketException.ThrowIfNotEqual((int)packet.Type, (int)PacketType.SERVERDATA_AUTH_RESPONSE);
                }

                // Throw exception if authentication failed
                if (packet.Id == -1 || packet.Id != id)
                {
                    _tcpClient.Close();
                    throw new AuthenticationException("Authentication failed. The server did not accept the provided password.");
                }
            }

            /// <summary>
            /// Sends a command to the server and waits for the response.
            /// </summary>
            /// <param name="command">The command to be sent to the server.</param>
            /// <returns>A task that represents the asynchronous operation. The task result contains the server response to the command.</returns>
            /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
            public async Task<string> SendCommand(string command)
            {
                // Send the command and a empty command packet
                int id = new Random().Next(4096), dummyId = id + 1;
                await _tcpClient.SendAsync(new Packet(id, PacketType.SERVERDATA_EXECCOMMAND, command).GetBytes());
                await _tcpClient.SendAsync(new Packet(dummyId, PacketType.SERVERDATA_EXECCOMMAND, string.Empty).GetBytes());

                List<Packet> packets;
                var bytes = new byte[0];
                var response = new StringBuilder();

                while (true)
                {
                    // Receive and concat to last unused bytes
                    bytes = bytes.Concat(await _tcpClient.ReceiveAsync()).ToArray();

                    // Get the packets and get the unused bytes
                    (packets, bytes) = GetPackets(bytes);

                    // Loop all packets
                    foreach (var packet in packets)
                    {
                        // Return the full response until reaching the empty command packet
                        if (packet.Id == dummyId)
                        {
                            return response.ToString();
                        }

                        // Append the body data to response
                        response.Append(packet.Body);
                    }
                }
            }

            /// <summary>
            /// Handle Multiple-packet Responses
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            private (List<Packet>, byte[]) GetPackets(byte[] bytes)
            {
                var packets = new List<Packet>();

                using (var br = new BinaryReader(new MemoryStream(bytes)))
                {
                    // + 4 to ensure br.ReadInt32() is readable
                    while (br.BaseStream.Position + 4 < br.BaseStream.Length)
                    {
                        int size = br.ReadInt32();

                        // Return if we know not enough bytes to read
                        if (br.BaseStream.Position + size > br.BaseStream.Length)
                        {
                            return (packets, bytes.Skip((int)br.BaseStream.Position - 4).ToArray());
                        }

                        // Read packet and append to packets
                        var id = br.ReadInt32();
                        var type = (PacketType)br.ReadInt32();
                        var body = br.ReadStringEx();
                        br.ReadByte();

                        packets.Add(new Packet(id, type, body));
                    }

                    return (packets, new byte[0]);
                }
            }

            private enum PacketType : int
            {
                SERVERDATA_AUTH = 3,
                SERVERDATA_AUTH_RESPONSE = 2,
                SERVERDATA_EXECCOMMAND = 2,
                SERVERDATA_RESPONSE_VALUE = 0,
            }

            private class Packet
            {
                public int Id { get; private set; }
                public PacketType Type { get; private set; }
                public string Body { get; private set; }

                public Packet(int id, PacketType type, string body)
                {
                    (Id, Type, Body) = (id, type, body);
                }

                public byte[] GetBytes()
                {
                    var idBytes = BitConverter.GetBytes(Id);
                    var typeBytes = BitConverter.GetBytes((int)Type);
                    var bodyBytes = Encoding.UTF8.GetBytes(Body + "\0");
                    var size = idBytes.Length + typeBytes.Length + bodyBytes.Length;

                    return BitConverter.GetBytes(size).Concat(idBytes).Concat(typeBytes).Concat(bodyBytes).ToArray();
                }

                /// <summary>
                /// Single-packet Responses
                /// </summary>
                /// <param name="bytes"></param>
                public Packet(byte[] bytes)
                {
                    using (var br = new BinaryReader(new MemoryStream(bytes)))
                    {
                        br.ReadInt32();
                        Id = br.ReadInt32();
                        Type = (PacketType)br.ReadInt32();
                        Body = br.ReadStringEx();
                    }
                }
            }
        }
    }
}

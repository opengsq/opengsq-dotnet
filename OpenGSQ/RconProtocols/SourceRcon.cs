using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;

namespace OpenGSQ.RconProtocols
{
    /// <summary>
    /// Source RCON Protocol
    /// </summary>
    public class SourceRcon : ProtocolBase, IDisposable
    {
        private System.Net.Sockets.TcpClient? _tcpClient;

        /// <inheritdoc/>
        public override string FullName => "Source RCON Protocol";

        /// <summary>
        /// Source RCON Protocol
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public SourceRcon(string address, int port = 27015, int timeout = 5000) : base(address, port, timeout)
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
            await _tcpClient.ConnectAsync(Host, Port, Timeout);
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
            if (_tcpClient == null)
            {
                throw new InvalidOperationException("The client is not authenticated. Please ensure that the Authenticate() method has been successfully called before attempting to send a command.");
            }

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
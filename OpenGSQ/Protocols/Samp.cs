using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Responses.Samp;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// San Andreas Multiplayer Protocol
    /// </summary>
    public class Samp : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "San Andreas Multiplayer Protocol";

        private static readonly byte[] _requestHeader = Encoding.UTF8.GetBytes("SAMP");
        private static readonly byte[] _responseHeader = Encoding.UTF8.GetBytes("SAMP");

        /// <summary>
        /// Initializes a new instance of the Samp class.
        /// </summary>
        /// <param name="host">The host address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Samp(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Asynchronously gets the status of the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a StatusResponse object with the server status.</returns>
        public async Task<Status> GetStatus()
        {
            var response = await SendAndReceive(new byte[] { (byte)'i' });
            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);

            return new Status
            {
                Password = br.ReadByte() == 1,
                NumPlayers = br.ReadInt16(),
                MaxPlayers = br.ReadInt16(),
                ServerName = ReadString(br, 4),
                GameType = ReadString(br, 4),
                Language = ReadString(br, 4),
            };
        }

        /// <summary>
        /// Asynchronously gets the list of players from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Player objects.</returns>
        public async Task<List<Player>> GetPlayers()
        {
            var response = await SendAndReceive(new byte[] { (byte)'d' });
            var players = new List<Player>();

            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);
            var numplayers = br.ReadInt16();

            for (var i = 0; i < numplayers; i++)
            {
                players.Add(new Player
                {
                    Id = br.ReadByte(),
                    Name = ReadString(br),
                    Score = br.ReadInt32(),
                    Ping = br.ReadInt32()
                });
            }

            return players;
        }

        /// <summary>
        /// Asynchronously gets the rules.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of rules.</returns>
        public async Task<Dictionary<string, string>> GetRules()
        {
            var response = await SendAndReceive(new byte[] { (byte)'r' });

            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);
            var numrules = br.ReadInt16();

            var rules = new Dictionary<string, string>();

            for (int i = 0; i < numrules; i++)
            {
                rules.Add(ReadString(br), ReadString(br));
            }

            return rules;
        }

        private async Task<byte[]> SendAndReceive(byte[] data)
        {
            // Format the address
            var host = (await GetIPEndPoint()).Address.ToString();

            byte[] headerData = new byte[6];
            string[] splitIp = host.Split('.');

            for (int i = 0; i < splitIp.Length; i++)
            {
                headerData[i] = Convert.ToByte(int.Parse(splitIp[i]));
            }

            headerData[4] = (byte)(Port >> 8);
            headerData[5] = (byte)Port;

            byte[] packetHeader = headerData.Concat(data).ToArray();
            var request = _requestHeader.Concat(packetHeader).ToArray();

            // Validate the response
            using var udpClient = new UdpClient();
            var response = await udpClient.CommunicateAsync(this, request);
            var header = response[.._responseHeader.Length];

            if (!header.SequenceEqual(_responseHeader))
            {
                throw new InvalidPacketException($"Packet header mismatch. Received: {BitConverter.ToString(header)}. Expected: {BitConverter.ToString(_responseHeader)}.");
            }

            return response[(_responseHeader.Length + packetHeader.Length)..];
        }

        private string ReadString(BinaryReader br, int readOffset = 1)
        {
            var length = readOffset == 1 ? br.ReadByte() : br.ReadInt32();
            return Encoding.UTF8.GetString(br.ReadBytes(length));
        }
    }
}

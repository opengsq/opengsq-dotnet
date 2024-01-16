using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Responses.Vcmp;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Vice City Multiplayer Protocol
    /// </summary>
    public class Vcmp : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Vice City Multiplayer Protocol";

        private static readonly byte[] _requestHeader = Encoding.UTF8.GetBytes("VCMP");
        private static readonly byte[] _responseHeader = Encoding.UTF8.GetBytes("MP04");

        /// <summary>
        /// Initializes a new instance of the Vcmp class.
        /// </summary>
        /// <param name="host">The host address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Vcmp(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Asynchronously gets the status of the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a StatusResponse object with the server status.</returns>
        public async Task<StatusResponse> GetStatus()
        {
            var response = await SendAndReceive(new byte[] { (byte)'i' });
            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);

            return new StatusResponse
            {
                Version = Encoding.UTF8.GetString(br.ReadBytes(12)).Trim('\0'),
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
            var response = await SendAndReceive(new byte[] { (byte)'c' });
            var players = new List<Player>();

            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);
            var numplayers = br.ReadInt16();

            for (var i = 0; i < numplayers; i++)
            {
                players.Add(new Player { Name = ReadString(br) });
            }

            return players;
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

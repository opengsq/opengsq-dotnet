using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// The request header to be sent to the remote host.
        /// </summary>
        protected byte[] RequestHeader = Encoding.UTF8.GetBytes("VCMP");

        /// <summary>
        /// The expected response header from the remote host.
        /// </summary>
        protected byte[] ResponseHeader = Encoding.UTF8.GetBytes("MP04");

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
        public async Task<Status> GetStatus()
        {
            var response = await SendAndReceive(new byte[] { (byte)'i' });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                return new Status
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
        }

        /// <summary>
        /// Asynchronously gets the list of players from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Player objects.</returns>
        public async Task<List<Player>> GetPlayers()
        {
            var response = await SendAndReceive(new byte[] { (byte)'c' });

            using (var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8))
            {
                var numplayers = br.ReadInt16();
                var players = new List<Player>();

                for (var i = 0; i < numplayers; i++)
                {
                    players.Add(new Player { Name = ReadString(br) });
                }

                return players;
            }
        }

        /// <summary>
        /// Sends data to a remote host and receives a response.
        /// </summary>
        /// <param name="data">The data to be sent.</param>
        /// <returns>A byte array containing the response from the remote host.</returns>
        protected async Task<byte[]> SendAndReceive(byte[] data)
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
            var request = RequestHeader.Concat(packetHeader).ToArray();

            // Validate the response
            var response = await UdpClient.CommunicateAsync(this, request);
            var header = response.Take(ResponseHeader.Length).ToArray();

            if (!header.SequenceEqual(ResponseHeader))
            {
                throw new InvalidPacketException($"Packet header mismatch. Received: {BitConverter.ToString(header)}. Expected: {BitConverter.ToString(ResponseHeader)}.");
            }

            return response.Skip(ResponseHeader.Length + packetHeader.Length).ToArray();
        }

        /// <summary>
        /// Reads a string from a binary reader.
        /// </summary>
        /// <param name="br">The binary reader.</param>
        /// <param name="readOffset">The read offset. Default is 1.</param>
        /// <returns>The string read from the binary reader.</returns>
        protected string ReadString(BinaryReader br, int readOffset = 1)
        {
            var length = readOffset == 1 ? br.ReadByte() : br.ReadInt32();
            return Encoding.UTF8.GetString(br.ReadBytes(length));
        }
    }
}

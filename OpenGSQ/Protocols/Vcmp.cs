using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        /// Retrieves the status of the server.
        /// </summary>
        /// <returns>A dictionary containing the server status.</returns>
        public Dictionary<string, object> GetStatus()
        {
            var response = SendAndReceive(new byte[] { (byte)'i' });

            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);
            var result = new Dictionary<string, object>
            {
                ["version"] = Encoding.UTF8.GetString(br.ReadBytes(12)).Trim('\0'),
                ["password"] = br.ReadByte(),
                ["numplayers"] = br.ReadInt16(),
                ["maxplayers"] = br.ReadInt16(),
                ["servername"] = ReadString(br, 4),
                ["gametype"] = ReadString(br, 4),
                ["language"] = ReadString(br, 4)
            };

            return result;
        }

        /// <summary>
        /// Retrieves the list of players on the server. The server may not respond when the number of players is greater than 100.
        /// </summary>
        /// <returns>A list of dictionaries containing player information.</returns>
        public List<Dictionary<string, string>> GetPlayers()
        {
            var response = SendAndReceive(new byte[] { (byte)'c' });
            var players = new List<Dictionary<string, string>>();

            using var br = new BinaryReader(new MemoryStream(response), Encoding.UTF8);
            var numplayers = br.ReadInt16();

            for (var i = 0; i < numplayers; i++)
            {
                var player = new Dictionary<string, string>
                {
                    ["name"] = ReadString(br)
                };
                players.Add(player);
            }

            return players;
        }

        private byte[] SendAndReceive(byte[] data)
        {
            // Format the address
            var host = Dns.GetHostEntry(IPEndPoint.Address).AddressList[0].ToString();

            byte[] headerData = new byte[6];
            string[] splitIp = host.Split('.');

            for (int i = 0; i < splitIp.Length; i++)
            {
                headerData[i] = Convert.ToByte(int.Parse(splitIp[i]));
            }

            headerData[4] = (byte)(IPEndPoint.Port >> 8);
            headerData[5] = (byte)IPEndPoint.Port;

            byte[] packetHeader = headerData.Concat(data).ToArray();
            var request = _requestHeader.Concat(packetHeader).ToArray();

            // Validate the response
            using var udpClient = new UdpClient();
            var response = udpClient.Communicate(this, request);
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

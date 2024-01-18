using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using OpenGSQ.Responses.RakNet;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// RakNet Protocol (https://wiki.vg/Raknet_Protocol)
    /// </summary>
    public class RakNet : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "RakNet Protocol";

        private static readonly byte[] ID_UNCONNECTED_PING = { 0x01 };
        private static readonly byte[] ID_UNCONNECTED_PONG = { 0x1C };
        private static readonly byte[] TIMESTAMP = { 0x12, 0x23, 0x34, 0x45, 0x56, 0x67, 0x78, 0x89 };
        private static readonly byte[] OFFLINE_MESSAGE_DATA_ID = { 0x00, 0xFF, 0xFF, 0x00, 0xFE, 0xFE, 0xFE, 0xFE, 0xFD, 0xFD, 0xFD, 0xFD, 0x12, 0x34, 0x56, 0x78 };
        private static readonly byte[] CLIENT_GUID = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Initializes a new instance of the RakNet class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout. Default is 5000.</param>
        public RakNet(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the server status asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server status.</returns>
        public async Task<Status> GetStatus()
        {
            var request = ID_UNCONNECTED_PING.Concat(TIMESTAMP).Concat(OFFLINE_MESSAGE_DATA_ID).Concat(CLIENT_GUID).ToArray();
            using var udpClient = new UdpClient();
            var response = await udpClient.CommunicateAsync(this, request);

            using var br = new BinaryReader(new MemoryStream(response));
            var header = br.ReadByte();

            if (header != ID_UNCONNECTED_PONG[0])
            {
                throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {ID_UNCONNECTED_PONG[0]}.");
            }

            br.ReadBytes(TIMESTAMP.Length + CLIENT_GUID.Length);  // skip timestamp and guid
            var magic = br.ReadBytes(OFFLINE_MESSAGE_DATA_ID.Length);

            if (!magic.SequenceEqual(OFFLINE_MESSAGE_DATA_ID))
            {
                throw new InvalidPacketException($"Magic value mismatch. Received: {magic}. Expected: {OFFLINE_MESSAGE_DATA_ID}.");
            }

            br.ReadInt16();  // skip remaining packet length

            byte[] delimiter = { (byte)';' };

            return new Status
            {
                Edition = br.ReadStringEx(delimiter),
                MotdLine1 = br.ReadStringEx(delimiter),
                ProtocolVersion = int.Parse(br.ReadStringEx(delimiter)),
                VersionName = br.ReadStringEx(delimiter),
                NumPlayers = int.Parse(br.ReadStringEx(delimiter)),
                MaxPlayers = int.Parse(br.ReadStringEx(delimiter)),
                ServerUniqueId = br.ReadStringEx(delimiter),
                MotdLine2 = br.ReadStringEx(delimiter),
                GameMode = br.ReadStringEx(delimiter),
                GameModeNumeric = int.Parse(br.ReadStringEx(delimiter)),
                PortIPv4 = int.Parse(br.ReadStringEx(delimiter)),
                PortIPv6 = int.Parse(br.ReadStringEx(delimiter))
            };
        }
    }
}

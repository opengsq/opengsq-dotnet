using System.Threading.Tasks;
using System.Linq;
using System.IO;
using OpenGSQ.Responses.RakNet;
using OpenGSQ.Exceptions;
using System.Buffers.Binary;
using System.Text;

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
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus()
        {
            byte[] request = ID_UNCONNECTED_PING.Concat(TIMESTAMP).Concat(OFFLINE_MESSAGE_DATA_ID).Concat(CLIENT_GUID).ToArray();
            byte[] response = await UdpClient.CommunicateAsync(this, request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, ID_UNCONNECTED_PONG[0]);

                br.ReadBytes(TIMESTAMP.Length + CLIENT_GUID.Length);  // skip timestamp and guid
                var magic = br.ReadBytes(OFFLINE_MESSAGE_DATA_ID.Length);
                InvalidPacketException.ThrowIfNotEqual(magic, OFFLINE_MESSAGE_DATA_ID);

                int length = BinaryPrimitives.ReverseEndianness(br.ReadUInt16());
                string[] splittedData = Encoding.UTF8.GetString(br.ReadBytes(length)).Split(';');

                var status = new Status
                {
                    Edition = splittedData[0],
                    MotdLine1 = splittedData[1],
                    VersionName = splittedData[3],
                    NumPlayers = int.Parse(splittedData[4]),
                    MaxPlayers = int.Parse(splittedData[5]),
                };

                status.ProtocolVersion = int.TryParse(splittedData[2], out int ProtocolVersionResult) ? ProtocolVersionResult : 0;

                try
                {
                    status.ServerUniqueId = splittedData[6];
                    status.MotdLine2 = splittedData[7];
                    status.GameMode = splittedData[8];
                    status.GameModeNumeric = int.TryParse(splittedData[9], out int GameModeNumericResult) ? GameModeNumericResult : 0;
                    status.PortIPv4 = int.Parse(splittedData[10]);
                    status.PortIPv6 = int.Parse(splittedData[11]);
                }
                catch
                {
                    // pass
                }

                return status;
            }
        }
    }
}

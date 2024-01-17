using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Status = OpenGSQ.Responses.KillingFloor.Status;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Killing Floor Protocol
    /// </summary>
    public class KillingFloor : Unreal2
    {
        /// <inheritdoc/>
        public override string FullName => "Killing Floor Protocol";

        /// <summary>
        /// Initializes a new instance of the KillingFloor class.
        /// </summary>
        public KillingFloor(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the details of the server.
        /// </summary>
        /// <returns>The details of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        public new async Task<Status> GetDetails()
        {
            using var udpClient = new UdpClient();
            byte[] response = await udpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, _DETAILS });

            // Remove the first 4 bytes \x80\x00\x00\x00
            BinaryReader br = new BinaryReader(new MemoryStream(response.Skip(4).ToArray()));
            byte header = br.ReadByte();

            if (header != _DETAILS)
            {
                throw new InvalidPacketException($"Packet header mismatch. Received: {header}. Expected: {_DETAILS}.");
            }

            var details = new Status
            {
                ServerId = br.ReadInt32(),
                ServerIP = br.ReadString(),
                GamePort = br.ReadInt32(),
                QueryPort = br.ReadInt32(),
                ServerName = ReadString(br),
                MapName = ReadString(br),
                GameType = ReadString(br),
                NumPlayers = br.ReadInt32(),
                MaxPlayers = br.ReadInt32(),
                WaveCurrent = br.ReadInt32(),
                WaveTotal = br.ReadInt32(),
                Ping = br.ReadInt32(),
                Flags = br.ReadInt32(),
                Skill = ReadString(br)
            };

            return details;
        }
    }
}
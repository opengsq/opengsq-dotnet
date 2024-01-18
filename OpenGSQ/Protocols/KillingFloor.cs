using System.IO;
using System.Threading.Tasks;
using OpenGSQ.Responses.KillingFloor;

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
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, _DETAILS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

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
}
using System.IO;
using System.Threading.Tasks;
using OpenGSQ.Responses.KillingFloor;
using OpenGSQ.Exceptions;

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
        /// <param name="stripColor">A boolean value indicating whether to strip color codes from the server name. Default is true.</param>
        /// <returns>The details of the server.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected value.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public new async Task<Status> GetDetails(bool stripColor = true)
        {
            byte[] response = await UdpClient.CommunicateAsync(this, new byte[] { 0x79, 0x00, 0x00, 0x00, DETAILS });

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                // Remove the first 4 bytes \x80\x00\x00\x00
                br.ReadBytes(4);

                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, DETAILS);

                return new Status
                {
                    ServerId = br.ReadInt32(),
                    ServerIP = br.ReadString(),
                    GamePort = br.ReadInt32(),
                    QueryPort = br.ReadInt32(),
                    ServerName = ReadString(br, stripColor),
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
            }
        }
    }
}
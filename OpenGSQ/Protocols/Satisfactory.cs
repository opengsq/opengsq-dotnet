using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;
using OpenGSQ.Responses.Satisfactory;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Satisfactory Protocol
    /// </summary>
    public class Satisfactory : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Satisfactory Protocol";

        /// <summary>
        /// Initializes a new instance of the Satisfactory class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port. Default is 15777.</param>
        /// <param name="timeout">The timeout. Default is 5000.</param>
        public Satisfactory(string host, int port = 15777, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the status asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the StatusResponse.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus()
        {
            // https://github.com/dopeghoti/SF-Tools/blob/main/Protocol.md
            byte[] request = new byte[] { 0, 0 }.Concat(Encoding.ASCII.GetBytes("opengsq")).ToArray();
            byte[] response = await UdpClient.CommunicateAsync(this, request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                byte header = br.ReadByte();
                InvalidPacketException.ThrowIfNotEqual(header, 1);

                br.ReadByte();  // Protocol version
                br.ReadBytes(8);  // Request data

                return new Status
                {
                    State = br.ReadByte(),
                    Version = br.ReadInt32(),
                    BeaconPort = br.ReadInt16()
                };
            }
        }
    }
}

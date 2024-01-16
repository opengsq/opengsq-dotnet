using System.Net;
using System.Threading.Tasks;

namespace OpenGSQ
{
    /// <summary>
    /// Abstract base class for protocols.
    /// </summary>
    public abstract class ProtocolBase
    {
        /// <summary>
        /// Gets the full name of the protocol.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Gets the host to connect to.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the port to connect to.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The timeout for the connection in seconds.
        /// </summary>
        public int Timeout { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProtocolBase class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The connection timeout in milliseconds. Default is 5 seconds.</param>
        public ProtocolBase(string host, int port, int timeout = 5000)
        {
            Host = host;
            Port = port;
            Timeout = timeout;
        }

        /// <summary>
        /// Asynchronously gets the Internet Protocol (IP) endpoint.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the IP endpoint.</returns>
        protected async Task<IPEndPoint> GetIPEndPoint()
        {
            if (IPAddress.TryParse(Host, out var ipAddress))
            {
                return new IPEndPoint(ipAddress, Port);
            }
            else
            {
                return new IPEndPoint((await Dns.GetHostAddressesAsync(Host))[0], Port);
            }
        }
    }
}

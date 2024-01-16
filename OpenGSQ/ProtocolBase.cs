using System.Net;

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
        /// The endpoint (IP address and port) of the server.
        /// </summary>
        public IPEndPoint IPEndPoint { get; private set; }

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
            if (IPAddress.TryParse(host, out var ipAddress))
            {
                IPEndPoint = new IPEndPoint(ipAddress, port);
            }
            else
            {
                IPEndPoint = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
            }

            Timeout = timeout;
        }
    }
}

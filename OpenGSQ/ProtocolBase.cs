using System.Net;

namespace OpenGSQ
{
    /// <summary>
    /// ProtocolBase class for Protocol
    /// </summary>
    public abstract class ProtocolBase
    {
        /// <summary>
        /// Represents a network endpoint as an IP address and a port number.
        /// </summary>
        protected IPEndPoint _EndPoint;

        /// <summary>
        /// Timeout in millisecond
        /// </summary>
        protected int _Timeout;

        /// <summary>
        /// Cached challenge bytes
        /// </summary>
        protected byte[] _Challenge = new byte[0];

        /// <summary>
        /// ProtocolBase
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public ProtocolBase(string address, int port, int timeout = 5000)
        {
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                _EndPoint = new IPEndPoint(ipAddress, port);
            }
            else
            {
                _EndPoint = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
            }

            _Timeout = timeout;
        }
    }
}

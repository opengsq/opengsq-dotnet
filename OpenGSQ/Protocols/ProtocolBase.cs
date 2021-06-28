using System.Net;

namespace OpenGSQ.Protocols
{
    public abstract class ProtocolBase
    {
        protected IPEndPoint EndPoint;
        protected int Timeout;

        public ProtocolBase(string address, int port, int timeout = 5000)
        {
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                EndPoint = new IPEndPoint(ipAddress, port);
            }
            else
            {
                EndPoint = new IPEndPoint(Dns.GetHostAddresses(address)[0], port);
            }

            Timeout = timeout;
        }
    }
}

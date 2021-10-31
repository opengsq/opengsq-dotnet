using System.Net;

namespace OpenGSQ
{
    public abstract class ProtocolBase
    {
        protected IPEndPoint _EndPoint;
        protected int _Timeout;
        protected byte[] _Challenge = new byte[0];

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

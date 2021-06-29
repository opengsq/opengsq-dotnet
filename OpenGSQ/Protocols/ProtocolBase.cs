﻿using System.Net;

namespace OpenGSQ.Protocols
{
    public abstract class ProtocolBase
    {
        protected IPEndPoint _EndPoint;
        protected int _Timeout;

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

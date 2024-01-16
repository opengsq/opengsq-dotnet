using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OpenGSQ
{
    /// <summary>
    /// A class that extends the UdpClient class with a Communicate method.
    /// </summary>
    public static class UdpClientExtensions
    {
        /// <summary>
        /// Sends data to a server and receives a response.
        /// </summary>
        /// <param name="udpClient">The UdpClient instance.</param>
        /// <param name="protocolBase">The protocol base containing the IP endpoint and timeout.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The response data received from the server.</returns>
        public static byte[] Communicate(this UdpClient udpClient, ProtocolBase protocolBase, byte[] data)
        {
            // Connect to the server
            udpClient.Connect(protocolBase.IPEndPoint);

            // Set the timeout
            udpClient.Client.SendTimeout = protocolBase.Timeout;
            udpClient.Client.ReceiveTimeout = protocolBase.Timeout;

            // Send the data
            udpClient.Send(data, data.Length);

            // Receive the response
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] responseData = udpClient.Receive(ref remoteEP);

            return responseData;
        }
    }

    /// <summary>
    /// A class that extends the TcpClient class with a Communicate method.
    /// </summary>
    public static class TcpClientExtensions
    {
        /// <summary>
        /// Sends data to a server and receives a response.
        /// </summary>
        /// <param name="tcpClient">The TcpClient instance.</param>
        /// <param name="protocolBase">The protocol base containing the IP endpoint and timeout.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The response data received from the server.</returns>
        public static byte[] Communicate(this TcpClient tcpClient, ProtocolBase protocolBase, byte[] data)
        {
            // Connect to the server
            tcpClient.Connect(protocolBase.IPEndPoint);

            // Set the timeout
            tcpClient.SendTimeout = protocolBase.Timeout;
            tcpClient.ReceiveTimeout = protocolBase.Timeout;

            // Get the stream object for writing and reading
            NetworkStream stream = tcpClient.GetStream();

            // Send the data
            stream.Write(data, 0, data.Length);

            byte[] bytesToRead = new byte[tcpClient.ReceiveBufferSize];
            stream.Read(bytesToRead, 0, tcpClient.ReceiveBufferSize);

            return bytesToRead;
        }
    }
}
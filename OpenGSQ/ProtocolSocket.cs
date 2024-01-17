using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OpenGSQ
{
    /// <summary>
    /// A class that extends the UdpClient class with a Communicate method.
    /// </summary>
    public static class UdpClientExtensions
    {
        /// <summary>
        /// Sends data to a connected UdpClient and returns the response.
        /// </summary>
        /// <param name="udpClient">The UdpClient to communicate with.</param>
        /// <param name="protocolBase">The protocol information.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public static async Task<byte[]> CommunicateAsync(this UdpClient udpClient, ProtocolBase protocolBase, byte[] data)
        {
            // Connect to the server
            udpClient.Connect(protocolBase.Host, protocolBase.Port);

            // Set the timeout
            udpClient.Client.SendTimeout = protocolBase.Timeout;
            udpClient.Client.ReceiveTimeout = protocolBase.Timeout;

            // Send the data
            await udpClient.SendAsync(data, data.Length);

            // Receive the response
            byte[] responseData = (await udpClient.ReceiveAsync()).Buffer;

            return responseData;
        }
    }

    /// <summary>
    /// A class that extends the TcpClient class with a Communicate method.
    /// </summary>
    public static class TcpClientExtensions
    {
        /// <summary>
        /// Sends data to a connected TcpClient and returns the response.
        /// </summary>
        /// <param name="tcpClient">The TcpClient to communicate with.</param>
        /// <param name="protocolBase">The protocol information.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public static async Task<byte[]> CommunicateAsync(this TcpClient tcpClient, ProtocolBase protocolBase, byte[] data)
        {
            // Connect to the server
            await tcpClient.ConnectAsync(protocolBase.Host, protocolBase.Port);

            // Set the timeout
            tcpClient.SendTimeout = protocolBase.Timeout;
            tcpClient.ReceiveTimeout = protocolBase.Timeout;

            // Send the data
            await tcpClient.SendAsync(data);

            return await tcpClient.ReceiveAsync();
        }

        /// <summary>
        /// Sends data to a connected TcpClient.
        /// </summary>
        /// <param name="tcpClient">The TcpClient to send data to.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SendAsync(this TcpClient tcpClient, byte[] data)
        {
            // Get the stream object for writing and reading
            NetworkStream stream = tcpClient.GetStream();

            // Send the data
            await stream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Receives data from a connected TcpClient.
        /// </summary>
        /// <param name="tcpClient">The TcpClient to receive data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the received data.</returns>
        public static async Task<byte[]> ReceiveAsync(this TcpClient tcpClient)
        {
            var buffer = new byte[tcpClient.Client.ReceiveBufferSize];
            var segment = new ArraySegment<byte>(buffer);
            var result = await tcpClient.Client.ReceiveAsync(segment, SocketFlags.None);

            var receivedBytes = new byte[result];
            Array.Copy(buffer, receivedBytes, result);

            return receivedBytes;
        }
    }
}
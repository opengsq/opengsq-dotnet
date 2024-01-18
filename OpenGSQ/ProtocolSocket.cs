using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OpenGSQ
{
    /// <summary>
    /// A class for handling UDP client communication.
    /// </summary>
    public static class UdpClient
    {
        /// <summary>
        /// Sends data to a connected UdpClient and returns the response.
        /// </summary>
        /// <param name="protocolBase">The protocol information.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public static async Task<byte[]> CommunicateAsync(ProtocolBase protocolBase, byte[] data)
        {
            using (var udpClient = new System.Net.Sockets.UdpClient())
            {
                // Set the timeout
                udpClient.Client.SendTimeout = protocolBase.Timeout;
                udpClient.Client.ReceiveTimeout = protocolBase.Timeout;

                // Connect to the server
                udpClient.Connect(protocolBase.Host, protocolBase.Port);

                // Send the data
                await udpClient.SendAsync(data, data.Length);

                // Receive the data
                return await udpClient.ReceiveAsyncWithTimeout();
            }
        }
    }

    /// <summary>
    /// A class that extends the UdpClient class with a Communicate method.
    /// </summary>
    public static class UdpClientExtensions
    {
        /// <summary>
        /// Receives a UDP datagram asynchronously with a timeout.
        /// </summary>
        /// <param name="udpClient">The UdpClient to receive from.</param>
        /// <returns>A byte array containing the received datagram.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public static async Task<byte[]> ReceiveAsyncWithTimeout(this System.Net.Sockets.UdpClient udpClient)
        {
            Task<UdpReceiveResult> receiveTask = udpClient.ReceiveAsync();

            if (await Task.WhenAny(receiveTask, Task.Delay(udpClient.Client.ReceiveTimeout)) == receiveTask)
            {
                // Task completed within timeout.
                return receiveTask.Result.Buffer;
            }
            else
            {
                // Task timed out.
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }

    /// <summary>
    /// A class for handling TCP client communication.
    /// </summary>
    public static class TcpClient
    {
        /// <summary>
        /// Sends data to a connected TcpClient and returns the response.
        /// </summary>
        /// <param name="protocolBase">The protocol information.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public static async Task<byte[]> CommunicateAsync(ProtocolBase protocolBase, byte[] data)
        {
            using (var tcpClient = new System.Net.Sockets.TcpClient())
            {
                // Set the timeout
                tcpClient.SendTimeout = protocolBase.Timeout;
                tcpClient.ReceiveTimeout = protocolBase.Timeout;

                // Connect to the server
                await tcpClient.ConnectAsync(protocolBase.Host, protocolBase.Port);

                // Send the data
                await tcpClient.SendAsync(data);

                return await tcpClient.ReceiveAsync();
            }
        }
    }

    /// <summary>
    /// A class that extends the TcpClient class with a Communicate method.
    /// </summary>
    public static class TcpClientExtensions
    {
        /// <summary>
        /// Sends data to a connected TcpClient.
        /// </summary>
        /// <param name="tcpClient">The TcpClient to send data to.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SendAsync(this System.Net.Sockets.TcpClient tcpClient, byte[] data)
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
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public static async Task<byte[]> ReceiveAsync(this System.Net.Sockets.TcpClient tcpClient)
        {
            var buffer = new byte[tcpClient.Client.ReceiveBufferSize];
            var segment = new ArraySegment<byte>(buffer);
            var receiveTask = tcpClient.Client.ReceiveAsync(segment, SocketFlags.None);

            if (await Task.WhenAny(receiveTask, Task.Delay(tcpClient.Client.ReceiveTimeout)) == receiveTask)
            {
                // Task completed within timeout.
                var receivedBytes = new byte[receiveTask.Result];
                Array.Copy(buffer, receivedBytes, receiveTask.Result);
                return receivedBytes;
            }
            else
            {
                // Task timed out.
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }
}
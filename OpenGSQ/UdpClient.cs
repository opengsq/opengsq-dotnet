using System.Net.Sockets;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;

namespace OpenGSQ
{
    /// <summary>
    /// A class for handling UDP client communication.
    /// </summary>
    internal static class UdpClient
    {
        /// <summary>
        /// Extension method for the UdpClient class to connect to a server with a specified timeout.
        /// </summary>
        /// <param name="udpClient">The UdpClient instance on which the extension method is invoked.</param>
        /// <param name="host">The host name or IP address of the remote server.</param>
        /// <param name="port">The port number of the remote server.</param>
        /// <param name="timeout">The send and receive timeout value for the UdpClient in milliseconds.</param>
        public static void Connect(this System.Net.Sockets.UdpClient udpClient, string host, int port, int timeout)
        {
            // Set the send timeout
            udpClient.Client.SendTimeout = timeout;

            // Set the receive timeout
            udpClient.Client.ReceiveTimeout = timeout;

            // Connect to the remote server
            udpClient.Connect(host, port);
        }

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
    internal static class UdpClientExtensions
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
}
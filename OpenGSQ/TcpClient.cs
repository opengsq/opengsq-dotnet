using System.Net.Sockets;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;

namespace OpenGSQ
{
    /// <summary>
    /// A class for handling TCP client communication.
    /// </summary>
    internal static class TcpClient
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
                // Connect to the server
                await tcpClient.ConnectAsync(protocolBase.Host, protocolBase.Port, protocolBase.Timeout);

                // Send the data
                await tcpClient.SendAsync(data);

                return await tcpClient.ReceiveAsync();
            }
        }
    }

    /// <summary>
    /// A class that extends the TcpClient class with a Communicate method.
    /// </summary>
    internal static class TcpClientExtensions
    {
        /// <summary>
        /// Extension method for the TcpClient class to connect to a server with a specified timeout.
        /// </summary>
        /// <param name="tcpClient">The TcpClient instance on which the extension method is invoked.</param>
        /// <param name="host">The host name or IP address of the remote server.</param>
        /// <param name="port">The port number of the remote server.</param>
        /// <param name="timeout">The send and receive timeout value for the TcpClient in milliseconds.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public static async Task ConnectAsync(this System.Net.Sockets.TcpClient tcpClient, string host, int port, int timeout)
        {
            // Set the timeout
            tcpClient.SendTimeout = timeout;
            tcpClient.ReceiveTimeout = timeout;

            var receiveTask = tcpClient.ConnectAsync(host, port);

            if (await Task.WhenAny(receiveTask, Task.Delay(timeout)) != receiveTask)
            {
                // Task timed out.
                throw new TimeoutException("The operation has timed out.");
            }
        }

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
            var segment = new System.ArraySegment<byte>(buffer);
            var receiveTask = tcpClient.Client.ReceiveAsync(segment, SocketFlags.None);

            if (await Task.WhenAny(receiveTask, Task.Delay(tcpClient.Client.ReceiveTimeout)) == receiveTask)
            {
                // Task completed within timeout.
                var receivedBytes = new byte[receiveTask.Result];
                System.Array.Copy(buffer, receivedBytes, receiveTask.Result);
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
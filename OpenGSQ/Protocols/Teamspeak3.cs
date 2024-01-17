using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Teamspeak 3 Protocol
    /// </summary>
    public class Teamspeak3 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Teamspeak 3 Protocol";

        private readonly int _voicePort;

        /// <summary>
        /// Initializes a new instance of the Teamspeak3 class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="voicePort">The voice port.</param>
        /// <param name="timeout">The timeout. Default is 5000.</param>
        public Teamspeak3(string host, int port, int voicePort, int timeout = 5000) : base(host, port, timeout)
        {
            _voicePort = voicePort;
        }

        /// <summary>
        /// Gets the server information asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server information.</returns>
        public async Task<Dictionary<string, string>> GetInfo()
        {
            var response = await SendAndReceive(Encoding.ASCII.GetBytes("serverinfo"));
            return ParseKvs(response);
        }

        /// <summary>
        /// Gets the client list asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the client list.</returns>
        public async Task<List<Dictionary<string, string>>> GetClients()
        {
            var response = await SendAndReceive(Encoding.ASCII.GetBytes("clientlist"));
            return ParseRows(response);
        }

        /// <summary>
        /// Gets the channel list asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the channel list.</returns>
        public async Task<List<Dictionary<string, string>>> GetChannels()
        {
            var response = await SendAndReceive(Encoding.ASCII.GetBytes("channellist -topic"));
            return ParseRows(response);
        }

        private async Task<string> SendAndReceive(byte[] data)
        {
            using var tcpClient = new TcpClient();

            // Set the timeout
            tcpClient.SendTimeout = Timeout;
            tcpClient.ReceiveTimeout = Timeout;

            await tcpClient.ConnectAsync(Host, Port);

            // Welcome message from the server
            await tcpClient.ReceiveAsync();

            // Send voice port
            var portData = Encoding.ASCII.GetBytes($"use port={_voicePort}\n");
            await tcpClient.SendAsync(portData);
            await tcpClient.ReceiveAsync();

            // Send data
            await tcpClient.SendAsync(data.Concat(new byte[] { 0x0A }).ToArray());

            // Receive response
            string response = string.Empty;

            while (!response.EndsWith("error id=0 msg=ok\n\r"))
            {
                response += Encoding.UTF8.GetString(await tcpClient.ReceiveAsync());
            }

            // Remove "\n\rerror id=0 msg=ok\n\r"
            return response[..^21];
        }

        private List<Dictionary<string, string>> ParseRows(string response)
        {
            return response.Split('|').Select(ParseKvs).ToList();
        }

        private Dictionary<string, string> ParseKvs(string response)
        {
            var kvs = new Dictionary<string, string>();

            foreach (var kv in response.Split(' '))
            {
                string[] items = kv.Split("=", 2);
                string key = items[0];
                string val = items.Length == 2 ? items[1] : "";
                kvs[key] = val.Replace("\\p", "|").Replace("\\s", " ").Replace("\\/", "/");
            }

            return kvs;
        }
    }
}

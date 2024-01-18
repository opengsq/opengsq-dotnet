using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using OpenGSQ.Responses.Scum;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Scum Protocol
    /// </summary>
    public class Scum : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Scum Protocol";

        private static readonly List<(string, int)> _masterServers = new List<(string, int)>
        {
            ("176.57.138.2", 1040),
            ("172.107.16.215", 1040),
            ("206.189.248.133", 1040)
        };

        /// <summary>
        /// Initializes a new instance of the Scum class.
        /// </summary>
        /// <param name="host">The host of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="timeout">The timeout for server requests.</param>
        public Scum(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        /// <param name="masterServers">The list of master servers to query.</param>
        /// <returns>The status of the server.</returns>
        /// <exception cref="ServerNotFoundException">Thrown when the server is not found in the list of master servers.</exception>
        public async Task<Status> GetStatus(List<Status> masterServers = null)
        {
            var ip = (await GetIPEndPoint()).Address.ToString();

            masterServers ??= await QueryMasterServers();

            foreach (var server in masterServers)
            {
                if (server.Ip == ip && server.Port == Port)
                {
                    return server;
                }
            }

            throw new ServerNotFoundException($"The server with IP address {ip} and port {Port} was not found in the list of master servers.");
        }

        /// <summary>
        /// Queries the master servers for a list of servers.
        /// </summary>
        /// <returns>A list of servers from the master servers.</returns>
        /// <exception cref="Exception">Thrown when failed to connect to any of the master servers.</exception>
        public static async Task<List<Status>> QueryMasterServers()
        {
            foreach (var (host, port) in _masterServers)
            {
                try
                {
                    using var tcpClient = new TcpClient();
                    tcpClient.ReceiveTimeout = 5000;
                    await tcpClient.ConnectAsync(host, port);
                    await tcpClient.SendAsync(new byte[] { 0x04, 0x03, 0x00, 0x00 });

                    var total = -1;
                    var response = new byte[0];
                    var servers = new List<Status>();

                    while (total == -1 || servers.Count < total)
                    {
                        response = response.Concat(await tcpClient.ReceiveAsync()).ToArray();
                        using var br = new BinaryReader(new MemoryStream(response));

                        // first packet return the total number of servers
                        if (total == -1)
                        {
                            total = br.ReadInt16();
                        }

                        // server bytes length always 127
                        while (br.BaseStream.Length - br.BaseStream.Position >= 127)
                        {
                            var statusResponse = new Status
                            {
                                Ip = string.Join(".", br.ReadBytes(4).Reverse().Select(b => b.ToString())),
                                Port = br.ReadInt16(),
                                Name = Encoding.UTF8.GetString(br.ReadBytes(100).TakeWhile(b => b != 0).ToArray())
                            };
                            br.ReadByte();  // skip
                            statusResponse.NumPlayers = br.ReadByte();
                            statusResponse.MaxPlayers = br.ReadByte();
                            statusResponse.Time = br.ReadByte();
                            br.ReadByte();  // skip
                            statusResponse.Password = ((br.ReadByte() >> 1) & 1) == 1;
                            br.ReadBytes(7);  // skip
                            var v = br.ReadBytes(8).Reverse().Select(b => Convert.ToInt32(b).ToString("X").PadLeft(2, '0')).ToList();
                            statusResponse.Version = $"{Convert.ToInt32(v[0], 16)}.{Convert.ToInt32(v[1], 16)}.{Convert.ToInt32(v[2] + v[3], 16)}.{Convert.ToInt32(v[4] + v[5] + v[6] + v[7], 16)}";
                            servers.Add(statusResponse);
                        }

                        // if the length is less than 127, save the unused bytes for next loop
                        response = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    }

                    return servers;
                }
                catch (SocketException)
                {
                    // Ignore exceptions
                }
            }

            throw new Exception("Failed to connect to any of the master servers.");
        }
    }
}

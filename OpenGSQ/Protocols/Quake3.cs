using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGSQ.Responses.Quake2;
using OpenGSQ.Exceptions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake3 Query Protocol
    /// </summary>
    public class Quake3 : Quake2
    {
        /// <inheritdoc/>
        public override string FullName => "Quake3 Query Protocol";

        /// <summary>
        /// Initializes a new instance of the Quake3 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Quake3(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            RequestHeader = "getstatus";
            ResponseHeader = "statusResponse\n";
        }

        /// <summary>
        /// Gets the server information. If stripColor is true, color codes are removed from the server name.
        /// </summary>
        /// <param name="stripColor">A boolean indicating whether to remove color codes from the server name.</param>
        /// <returns>A dictionary containing the server information.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Dictionary<string, string>> GetInfo(bool stripColor = true)
        {
            byte[] response = await ConnectAndSend("getinfo");

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var header = br.ReadStringEx(Delimiter1);
                InvalidPacketException.ThrowIfNotEqual(header, "infoResponse\n");

                var info = ParseInfo(br);

                if (stripColor)
                {
                    if (info.ContainsKey("hostname"))
                    {
                        info["hostname"] = StripColors(info["hostname"]);
                    }
                }

                return info;
            }
        }

        /// <summary>
        /// Gets the status of the server including information and players. If stripColor is true, color codes are removed from the server name and player names.
        /// </summary>
        /// <param name="stripColor">A boolean indicating whether to remove color codes from the server name and player names.</param>
        /// <returns>A Status object containing the server information and players.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus(bool stripColor = true)
        {
            using (var br = await GetResponseBinaryReader())
            {
                var status = new Status
                {
                    Info = ParseInfo(br),
                    Players = ParsePlayers(br),
                };

                if (stripColor)
                {
                    if (status.Info.ContainsKey("sv_hostname"))
                    {
                        status.Info["sv_hostname"] = StripColors(status.Info["sv_hostname"]);
                    }

                    foreach (var player in status.Players)
                    {
                        if (player.Name != null)
                        {
                            player.Name = StripColors(player.Name);
                        }
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// Removes color codes from a string.
        /// </summary>
        /// <param name="text">The text to remove color codes from.</param>
        /// <returns>The text with color codes removed.</returns>
        public static string StripColors(string text)
        {
            return new Regex("\\^(X.{6}|.)").Replace(text, string.Empty);
        }
    }
}

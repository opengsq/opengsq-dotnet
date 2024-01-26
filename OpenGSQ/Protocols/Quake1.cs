using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGSQ.Exceptions;
using OpenGSQ.Responses.Quake1;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake1 Query Protocol
    /// </summary>
    public class Quake1 : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Quake1 Query Protocol";

        /// <summary>
        /// The ASCII value of the backslash character used as a delimiter.
        /// </summary>
        protected byte Delimiter1 = Encoding.ASCII.GetBytes("\\")[0];

        /// <summary>
        /// The ASCII value of the newline character used as a delimiter.
        /// </summary>
        protected byte Delimiter2 = Encoding.ASCII.GetBytes("\n")[0];

        /// <summary>
        /// The header of the request.
        /// </summary>
        protected string RequestHeader;

        /// <summary>
        /// The header of the response.
        /// </summary>
        protected string ResponseHeader;

        /// <summary>
        /// Initializes a new instance of the Quake1 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Quake1(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            RequestHeader = "status";
            ResponseHeader = "n";
        }

        /// <summary>
        /// Gets the status of the server including information and players.
        /// </summary>
        /// <returns>A Status object containing the server information and players.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Status> GetStatus()
        {
            using (var br = await GetResponseBinaryReader())
            {
                return new Status
                {
                    Info = ParseInfo(br),
                    Players = ParsePlayers(br),
                };
            }
        }

        /// <summary>
        /// Gets a BinaryReader for the response data.
        /// </summary>
        /// <returns>A BinaryReader for the response data.</returns>
        /// <exception cref="InvalidPacketException">Thrown when the packet header does not match the expected header.</exception>
        protected async Task<BinaryReader> GetResponseBinaryReader()
        {
            byte[] response = await ConnectAndSend(RequestHeader);

            var br = new BinaryReader(new MemoryStream(response));
            var header = br.ReadStringEx(Delimiter1);
            InvalidPacketException.ThrowIfNotEqual(header, ResponseHeader);

            return br;
        }

        /// <summary>
        /// Parses the server information from the BinaryReader.
        /// </summary>
        /// <param name="br">The BinaryReader containing the server information.</param>
        /// <returns>A dictionary containing the server information.</returns>
        protected Dictionary<string, string> ParseInfo(BinaryReader br)
        {
            var info = new Dictionary<string, string>();

            // Read all key values until meet \n
            while (br.TryReadStringEx(out var key, Delimiter1))
            {
                info[key] = br.ReadStringEx(new byte[] { Delimiter1, Delimiter2 });

                br.BaseStream.Position--;

                if (br.ReadByte() == Delimiter2)
                {
                    break;
                }
            }

            return info;
        }

        /// <summary>
        /// Parses the player information from the BinaryReader.
        /// </summary>
        /// <param name="br">The BinaryReader containing the player information.</param>
        /// <returns>A list of Player objects.</returns>
        protected List<Player> ParsePlayers(BinaryReader br)
        {
            var players = new List<Player>();

            foreach (var matches in GetPlayerMatchCollections(br))
            {
                players.Add(new Player
                {
                    Id = int.Parse(matches[0].Value),
                    Score = int.Parse(matches[1].Value),
                    Time = int.Parse(matches[2].Value),
                    Ping = int.Parse(matches[3].Value),
                    Name = matches[4].Value.Trim('"'),
                    Skin = matches[5].Value.Trim('"'),
                    Color1 = int.Parse(matches[6].Value),
                    Color2 = int.Parse(matches[7].Value),
                });
            }

            return players;
        }

        /// <summary>
        /// Gets a list of MatchCollection objects for each player.
        /// </summary>
        /// <param name="br">The BinaryReader containing the player information.</param>
        /// <returns>A list of MatchCollection objects for each player.</returns>
        protected List<MatchCollection> GetPlayerMatchCollections(BinaryReader br)
        {
            var matchCollections = new List<MatchCollection>();

            // Regex to split with whitespace and double quote
            var regex = new Regex("\"(\\\"|[^\"])*?\"|[^\\s]+");

            // Read all players
            while (br.BaseStream.Position < br.BaseStream.Length && br.TryReadStringEx(out var playerInfo, Delimiter2))
            {
                matchCollections.Add(regex.Matches(playerInfo));
            }

            return matchCollections;
        }

        /// <summary>
        /// Connects to the server and sends a request.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The response data received from the server.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        protected async Task<byte[]> ConnectAndSend(string request)
        {
            // Send Request
            var header = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            var requestData = new byte[0].Concat(header).Concat(Encoding.ASCII.GetBytes(request)).Concat(new byte[] { 0x00 }).ToArray();

            // Server response
            byte[] response = await UdpClient.CommunicateAsync(this, requestData);

            // Remove the last 0x00 if exists (Only if Quake1)
            if (response[response.Length - 1] == 0)
            {
                response = response.Take(response.Length - 1).ToArray();
            }

            // Add \n at the last of responseData if not exists
            if (response[response.Length - 1] != Delimiter2)
            {
                response = response.Concat(new byte[] { Delimiter2 }).ToArray();
            }

            // Remove the first four 0xFF
            return response.Skip(header.Length).ToArray();
        }
    }
}

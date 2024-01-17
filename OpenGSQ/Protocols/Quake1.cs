using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGSQ.Responses.Quake1;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Quake1 Query Protocol
    /// </summary>
    public class Quake1 : ProtocolBase
    {
        /// <summary>
        /// The ASCII value of the backslash character used as a delimiter.
        /// </summary>
        protected byte _Delimiter1 = Encoding.ASCII.GetBytes("\\")[0];

        /// <summary>
        /// The ASCII value of the newline character used as a delimiter.
        /// </summary>
        protected byte _Delimiter2 = Encoding.ASCII.GetBytes("\n")[0];

        /// <summary>
        /// The header of the request.
        /// </summary>
        protected string _RequestHeader;

        /// <summary>
        /// The header of the response.
        /// </summary>
        protected string _ResponseHeader;

        /// <inheritdoc/>
        public override string FullName => "Quake1 Query Protocol";

        /// <summary>
        /// Initializes a new instance of the Quake1 class.
        /// </summary>
        /// <param name="host">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Quake1(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            _RequestHeader = "status";
            _ResponseHeader = "n";
        }

        /// <summary>
        /// Gets the status of the server including information and players.
        /// </summary>
        /// <returns>A Status object containing the server information and players.</returns>
        /// <exception cref="SocketException">Thrown when a socket error occurs.</exception>
        public async Task<StatusResponse> GetStatus()
        {
            using var br = await GetResponseBinaryReader();

            return new StatusResponse
            {
                Info = ParseInfo(br),
                Players = ParsePlayers(br),
            };
        }

        /// <summary>
        /// Gets a BinaryReader for the response data.
        /// </summary>
        /// <returns>A BinaryReader for the response data.</returns>
        /// <exception cref="Exception">Thrown when the packet header does not match the expected header.</exception>
        protected async Task<BinaryReader> GetResponseBinaryReader()
        {
            var responseData = await ConnectAndSend(_RequestHeader);

            var br = new BinaryReader(new MemoryStream(responseData), Encoding.UTF8);
            var header = br.ReadStringEx(_Delimiter1);

            if (header != _ResponseHeader)
            {
                throw new Exception($"Packet header mismatch. Received: {header}. Expected: {_ResponseHeader}.");
            }

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
            while (br.TryReadStringEx(out var key, _Delimiter1))
            {
                info[key] = br.ReadStringEx(new byte[] { _Delimiter1, _Delimiter2 });

                br.BaseStream.Position--;

                if (br.ReadByte() == _Delimiter2)
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
            while (br.BaseStream.Position < br.BaseStream.Length && br.TryReadStringEx(out var playerInfo, _Delimiter2))
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
        protected async Task<byte[]> ConnectAndSend(string request)
        {
            using var udpClient = new UdpClient();
            var header = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            // Send Request
            var requestData = new byte[0].Concat(header).Concat(Encoding.ASCII.GetBytes(request)).Concat(new byte[] { 0x00 }).ToArray();

            // Server response
            var responseData = await udpClient.CommunicateAsync(this, requestData);

            // Remove the last 0x00 if exists (Only if Quake1)
            if (responseData[responseData.Length - 1] == 0)
            {
                responseData = responseData.Take(responseData.Length - 1).ToArray();
            }

            // Add \n at the last of responseData if not exists
            if (responseData[responseData.Length - 1] != _Delimiter2)
            {
                responseData = responseData.Concat(new byte[] { _Delimiter2 }).ToArray();
            }

            // Remove the first four 0xFF
            return responseData.Skip(header.Length).ToArray();
        }
    }
}

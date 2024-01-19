using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenGSQ.Responses.Samp;
using OpenGSQ.Exceptions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// San Andreas Multiplayer Protocol
    /// </summary>
    public class Samp : Vcmp
    {
        /// <inheritdoc/>
        public override string FullName => "San Andreas Multiplayer Protocol";

        /// <summary>
        /// Initializes a new instance of the Samp class.
        /// </summary>
        /// <param name="host">The host address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="timeout">The timeout for the connection in milliseconds.</param>
        public Samp(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
            RequestHeader = Encoding.UTF8.GetBytes("SAMP");
            ResponseHeader = Encoding.UTF8.GetBytes("SAMP");
        }

        /// <summary>
        /// Asynchronously gets the status of the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a StatusResponse object with the server status.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public new async Task<Status> GetStatus()
        {
            var response = await SendAndReceive((byte)'i');

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                return new Status
                {
                    Password = br.ReadByte() == 1,
                    NumPlayers = br.ReadInt16(),
                    MaxPlayers = br.ReadInt16(),
                    ServerName = ReadString(br, 4),
                    GameType = ReadString(br, 4),
                    Language = ReadString(br, 4),
                };
            }
        }

        /// <summary>
        /// Asynchronously gets the list of players from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Player objects.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public new async Task<List<Player>> GetPlayers()
        {
            var response = await SendAndReceive((byte)'d');

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var numplayers = br.ReadInt16();
                var players = new List<Player>();

                for (var i = 0; i < numplayers; i++)
                {
                    players.Add(new Player
                    {
                        Id = br.ReadByte(),
                        Name = ReadString(br),
                        Score = br.ReadInt32(),
                        Ping = br.ReadInt32()
                    });
                }

                return players;
            }
        }

        /// <summary>
        /// Asynchronously gets the rules.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of rules.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Dictionary<string, string>> GetRules()
        {
            var response = await SendAndReceive((byte)'r');

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                var numrules = br.ReadInt16();
                var rules = new Dictionary<string, string>();

                for (int i = 0; i < numrules; i++)
                {
                    rules.Add(ReadString(br), ReadString(br));
                }

                return rules;
            }
        }
    }
}

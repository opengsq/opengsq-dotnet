using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenGSQ.Responses.Battlefield;
using OpenGSQ.Exceptions;
using System.Globalization;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Battlefield Protocol
    /// </summary>
    public class Battlefield : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Battlefield Protocol";

        private readonly byte[] _info = new byte[] { 0x00, 0x00, 0x00, 0x21, 0x1b, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x00, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x49, 0x6e, 0x66, 0x6f, 0x00 };
        private readonly byte[] _version = new byte[] { 0x00, 0x00, 0x00, 0x22, 0x18, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6f, 0x6e, 0x00 };
        private readonly byte[] _players = new byte[] { 0x00, 0x00, 0x00, 0x23, 0x24, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x00, 0x6c, 0x69, 0x73, 0x74, 0x50, 0x6c, 0x61, 0x79, 0x65, 0x72, 0x73, 0x00, 0x03, 0x00, 0x00, 0x00, 0x61, 0x6c, 0x6c, 0x00 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Battlefield"/> class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The timeout for the connection.</param>
        public Battlefield(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        /// <summary>
        /// Retrieves the information about the game.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the information about the game.
        /// </returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<Info> GetInfo()
        {
            var data = new Stack<string>((await GetData(_info)).AsEnumerable().Reverse());

            var info = new Info
            {
                Hostname = data.Pop().Trim(),
                NumPlayers = int.Parse(data.Pop()),
                MaxPlayers = int.Parse(data.Pop()),
                GameType = data.Pop(),
                Map = data.Pop(),
                RoundsPlayed = int.Parse(data.Pop()),
                RoundsTotal = int.Parse(data.Pop()),
                Teams = new List<float>()
            };

            int numTeams = int.Parse(data.Pop());

            for (int i = 0; i < numTeams; i++)
            {
                info.Teams.Add(float.Parse(data.Pop(), CultureInfo.InvariantCulture));
            }

            info.TargetScore = int.Parse(data.Pop());
            info.Status = data.Pop();
            info.Ranked = data.Pop() == "true";
            info.PunkBuster = data.Pop() == "true";
            info.Password = data.Pop() == "true";
            info.Uptime = int.Parse(data.Pop());
            info.RoundTime = int.Parse(data.Pop());

            try
            {
                if (data.Peek() == "BC2")
                {
                    info.Mod = data.Pop();
                    data.Pop();
                }

                info.IpPort = data.Pop();
                info.PunkBusterVersion = data.Pop();
                info.JoinQueue = data.Pop() == "true";
                info.Region = data.Pop();
                info.PingSite = data.Pop();
                info.Country = data.Pop();

                try
                {
                    info.BlazePlayerCount = int.Parse(data.Peek());
                    info.BlazeGameState = data.ElementAt(1);
                }
                catch
                {
                    info.QuickMatch = data.Pop() == "true";
                }
            }
            catch
            {
                // pass
            }

            return info;
        }

        /// <summary>
        /// Retrieves the version information of the game mod.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the version information of the game mod.
        /// </returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<VersionInfo> GetVersion()
        {
            var data = await GetData(_version);

            return new VersionInfo
            {
                Mod = data[0],
                Version = data[1]
            };
        }

        /// <summary>
        /// Gets the players on the server.
        /// </summary>
        /// <returns>A list of dictionaries containing player information.</returns>
        /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
        public async Task<List<Dictionary<string, string>>> GetPlayers()
        {
            var data = await GetData(_players);
            var count = int.Parse(data[0]);
            var fields = data.GetRange(1, count);
            var numplayers = int.Parse(data[count + 1]);
            var players = new List<Dictionary<string, string>>();

            for (var i = 0; i < numplayers; i++)
            {
                var values = data.GetRange(count + 2 + i * count, count);
                var player = new Dictionary<string, string>();

                for (var j = 0; j < count; j++)
                {
                    player[fields[j]] = values[j];
                }

                players.Add(player);
            }

            return players;
        }

        private async Task<List<string>> GetData(byte[] request)
        {
            byte[] response = await TcpClient.CommunicateAsync(this, request);

            using (var br = new BinaryReader(new MemoryStream(response)))
            {
                br.ReadInt32();  // header
                br.ReadInt32();  // packet length
                var count = br.ReadInt32();  // string count
                var data = new List<string>();

                for (var i = 0; i < count; i++)
                {
                    br.ReadInt32();  // length of the string
                    data.Add(br.ReadStringEx());
                }

                return data.GetRange(1, data.Count - 1);
            }
        }
    }
}
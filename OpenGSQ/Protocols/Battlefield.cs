using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        /// Gets the status of the server.
        /// </summary>
        /// <returns>A dictionary containing the server status.</returns>
        public async Task<Dictionary<string, object>> GetInfo()
        {
            var data = new Stack<string>((await GetData(_info)).AsEnumerable().Reverse());

            var info = new Dictionary<string, object>
            {
                ["hostname"] = data.Pop().Trim(),
                ["numplayers"] = int.Parse(data.Pop()),
                ["maxplayers"] = int.Parse(data.Pop()),
                ["gametype"] = data.Pop(),
                ["map"] = data.Pop(),
                ["roundsplayed"] = int.Parse(data.Pop()),
                ["roundstotal"] = int.Parse(data.Pop()),
            };

            var teams = new List<float>();
            int numTeams = int.Parse(data.Pop());

            for (int i = 0; i < numTeams; i++)
            {
                teams.Add(float.Parse(data.Pop()));
            }

            info.Concat(new Dictionary<string, object>
            {
                ["teams"] = teams,
                ["targetscore"] = int.Parse(data.Pop()),
                ["status"] = data.Pop(),
                ["ranked"] = data.Pop() == "true",
                ["punkbuster"] = data.Pop() == "true",
                ["password"] = data.Pop() == "true",
                ["uptime"] = int.Parse(data.Pop()),
                ["roundtime"] = int.Parse(data.Pop())
            });

            try
            {
                if (data.Peek() == "BC2")
                {
                    info["mod"] = data.Pop();
                    data.Pop();
                }

                info["ip_port"] = data.Pop();
                info["punkbuster_version"] = data.Pop();
                info["join_queue"] = data.Pop() == "true";
                info["region"] = data.Pop();
                info["pingsite"] = data.Pop();
                info["country"] = data.Pop();

                try
                {
                    info["blaze_player_count"] = int.Parse(data.Peek());
                    info["blaze_game_state"] = data.ElementAt(1);
                }
                catch
                {
                    info["quickmatch"] = data.Pop() == "true";
                }
            }
            catch
            {
                // pass
            }

            return info;
        }

        /// <summary>
        /// Gets the version of the server.
        /// </summary>
        /// <returns>A dictionary containing the server version.</returns>
        public async Task<Dictionary<string, string>> GetVersion()
        {
            var data = await GetData(_version);
            return new Dictionary<string, string> { ["mod"] = data[0], ["version"] = data[1] };
        }

        /// <summary>
        /// Gets the players on the server.
        /// </summary>
        /// <returns>A list of dictionaries containing player information.</returns>
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
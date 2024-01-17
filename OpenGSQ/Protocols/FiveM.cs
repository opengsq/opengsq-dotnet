using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// FiveM Protocol (https://docs.fivem.net/docs/server-manual/proxy-setup/)
    /// </summary>
    public class FiveM : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "FiveM Protocol";

        /// <summary>
        /// Initializes a new instance of the FiveM class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The connection timeout in milliseconds. Default is 5 seconds.</param>
        public FiveM(string host, int port, int timeout = 5000) : base(host, port, timeout)
        {
        }

        private async Task<T> GetAsync<T>(string filename)
        {
            string url = $"http://{Host}:{Port}/{filename}.json?v={new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}";

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        /// <summary>
        /// Asynchronously gets information about the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the server information.</returns>
        public Task<Dictionary<string, object>> GetInfo()
        {
            return GetAsync<Dictionary<string, object>>("info");
        }

        /// <summary>
        /// Asynchronously gets the list of players from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of players.</returns>
        public Task<List<object>> GetPlayers()
        {
            return GetAsync<List<object>>("players");
        }

        /// <summary>
        /// Asynchronously gets dynamic information from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the dynamic information.</returns>
        public Task<Dictionary<string, object>> GetDynamic()
        {
            return GetAsync<Dictionary<string, object>>("dynamic");
        }
    }
}

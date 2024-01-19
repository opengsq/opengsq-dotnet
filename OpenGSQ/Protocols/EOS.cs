using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Linq;
using OpenGSQ.Responses.EOS;
using OpenGSQ.Exceptions;

namespace OpenGSQ.Protocols
{
    /// <summary>
    /// Epic Online Services (EOS) Protocol
    /// </summary>
    public class EOS : ProtocolBase
    {
        /// <inheritdoc/>
        public override string FullName => "Epic Online Services (EOS) Protocol";

        private readonly string _apiUrl = "https://api.epicgames.dev";
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _deploymentId;
        private string _accessToken;

        /// <summary>
        /// Initializes a new instance of the EOS class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="timeout">The connection timeout in milliseconds. Default is 5 seconds.</param>
        /// <param name="clientId">The client ID for authentication.</param>
        /// <param name="clientSecret">The client secret for authentication.</param>
        /// <param name="deploymentId">The deployment ID for authentication.</param>
        public EOS(string host, int port, int timeout = 5000, string clientId = null, string clientSecret = null, string deploymentId = null) : base(host, port, timeout)
        {
            if (clientId == null || clientSecret == null || deploymentId == null)
            {
                throw new ArgumentException("clientId, clientSecret, and deploymentId must not be null");
            }

            _clientId = clientId;
            _clientSecret = clientSecret;
            _deploymentId = deploymentId;
        }

        /// <summary>
        /// Asynchronously gets an access token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
        protected async Task<string> GetAccessTokenAsync()
        {
            string url = $"{_apiUrl}/auth/v1/oauth/token";
            string body = $"grant_type=client_credentials&deployment_id={_deploymentId}";
            string authInfo = $"{_clientId}:{_clientSecret}";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(url),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Basic", authInfo)
                }
            })
            {
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                return data["access_token"].ToString();
            }
        }

        /// <summary>
        /// Retrieves matchmaking data asynchronously.
        /// </summary>
        /// <param name="data">The data to be sent to the server.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the matchmaking data.
        /// </returns>
        /// <exception cref="AuthenticationException">Thrown when there is a failure in getting the access token.</exception>
        public async Task<Matchmaking> GetMatchmakingAsync(Dictionary<string, object> data)
        {
            if (_accessToken == null)
            {
                try
                {
                    _accessToken = await GetAccessTokenAsync();
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException($"Failed to get access token due to an error: {ex.Message}");
                }
            }

            string url = $"{_apiUrl}/matchmaking/v1/{_deploymentId}/filter";

            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(url),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _accessToken)
                }
            })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadFromJsonAsync<Matchmaking>();

                return responseData;
            }
        }

        /// <summary>
        /// Retrieves the information about the game server.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a dictionary of the server information.
        /// </returns>
        /// <exception cref="ServerNotFoundException">Thrown when the server is not found.</exception>
        /// <exception cref="AuthenticationException">Thrown when there is a failure in getting the access token.</exception>
        public async Task<Dictionary<string, object>> GetInfo()
        {
            string address = await GetIPAddress();
            string addressBoundPort = $":{Port}";

            var data = await GetMatchmakingAsync(new Dictionary<string, object>
            {
                { "criteria", new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "key", "attributes.ADDRESS_s" },
                            { "op", "EQUAL" },
                            { "value", address }
                        },
                        new Dictionary<string, object>
                        {
                            { "key", "attributes.ADDRESSBOUND_s" },
                            { "op", "CONTAINS" },
                            { "value", addressBoundPort }
                        },
                    }
                }
            });

            if (data.Count <= 0)
            {
                throw new ServerNotFoundException($"Server with address {address} and port {Port} was not found.");
            }

            return data.Sessions.First();
        }
    }
}

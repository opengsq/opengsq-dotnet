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

        private static readonly string _apiUrl = "https://api.epicgames.dev";
        private readonly string _deploymentId;
        private readonly string _accessToken;

        /// <summary>
        /// Initializes a new instance of the EOS class.
        /// </summary>
        /// <param name="host">The host name of the server.</param>
        /// <param name="port">The port number of the server.</param>
        /// <param name="deploymentId">The deployment ID for the application.</param>
        /// <param name="accessToken">The access token for the application.</param>
        /// <param name="timeout">The timeout value for the connection, in milliseconds. Default is 5000.</param>
        /// <exception cref="ArgumentException">Thrown when either deploymentId or accessToken is null.</exception>
        public EOS(string host, int port, string deploymentId,string accessToken, int timeout = 5000) : base(host, port, timeout)
        {
            if (deploymentId == null || accessToken == null)
            {
                throw new ArgumentException("deploymentId, and accessToken must not be null");
            }

            _deploymentId = deploymentId;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Asynchronously gets an access token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
        public static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string deploymentId, string grantType, string externalAuthType, string externalAuthToken)
        {
            string url = $"{_apiUrl}/auth/v1/oauth/token";

            var values = new Dictionary<string, string>
            {
                { "grant_type", grantType },
                { "external_auth_type", externalAuthType },
                { "external_auth_token", externalAuthToken },
                { "nonce", "opengsq" },
                { "deployment_id", deploymentId },
                { "display_name", "User" },
            };

            var content = new FormUrlEncodedContent(values);
            var queryString = await content.ReadAsStringAsync();

            string authInfo = $"{clientId}:{clientSecret}";
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
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(queryString, Encoding.UTF8, "application/x-www-form-urlencoded"));
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                if (data == null || !data.TryGetValue("access_token", out var accessToken))
                {
                    throw new Exception($"Failed to get access token from {url}");
                }

                return accessToken.ToString()!;
            }
        }

        /// <summary>
        /// Asynchronously retrieves an external authentication token.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="externalAuthType">The type of external authentication.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
        /// <exception cref="ArgumentException">Thrown when either clientId or clientSecret is null.</exception>
        /// <exception cref="NotImplementedException">Thrown when the provided externalAuthType hasn't been implemented yet.</exception>
        public static async Task<string> GetExternalAuthTokenAsync(string clientId, string clientSecret, string externalAuthType)
        {
            if (externalAuthType == "deviceid_access_token")
            {
                string url = $"{_apiUrl}/auth/v1/accounts/deviceid";

                var values = new Dictionary<string, string>
                {
                    { "deviceModel", "PC" },
                };

                var content = new FormUrlEncodedContent(values);
                var queryString = await content.ReadAsStringAsync();

                string authInfo = $"{clientId}:{clientSecret}";
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
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(queryString, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();

                    var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                    if (data == null || !data.TryGetValue("access_token", out var accessToken))
                    {
                        throw new Exception($"Failed to get access token from {url}");
                    }

                    return accessToken.ToString()!;
                }
            }

            throw new NotImplementedException($"The external authentication type '{externalAuthType}' is not supported. Please provide a supported authentication type.");
        }

        /// <summary>
        /// Asynchronously retrieves matchmaking data without any filter parameters.
        /// </summary>
        /// <param name="deploymentId">The deployment ID.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the matchmaking data.</returns>
        public static Task<Matchmaking> GetMatchmakingAsync(string deploymentId, string accessToken)
        {
            return GetMatchmakingAsync(deploymentId, accessToken, new Dictionary<string, object>());
        }

        /// <summary>
        /// Asynchronously retrieves matchmaking data.
        /// </summary>
        /// <param name="deploymentId">The deployment ID.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="filter">The filter parameters for the matchmaking request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the matchmaking data.</returns>
        public static async Task<Matchmaking> GetMatchmakingAsync(string deploymentId, string accessToken, Dictionary<string, object> filter)
        {
            string url = $"{_apiUrl}/matchmaking/v1/{deploymentId}/filter";

            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(url),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                }
            })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(filter), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadFromJsonAsync<Matchmaking>();

                return responseData ?? throw new Exception($"Failed to load data from {url}"); ;
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

            var data = await GetMatchmakingAsync(_deploymentId, _accessToken, new Dictionary<string, object>
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

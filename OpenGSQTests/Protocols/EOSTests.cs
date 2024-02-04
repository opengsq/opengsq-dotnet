using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class EOSTests : TestBase
    {
        public EOSTests() : base(typeof(EOSTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetMatchmakingAsyncTest()
        {
            // Palworld
            string clientId = "xyza78916PZ5DF0fAahu4tnrKKyFpqRE";
            string clientSecret = "j0NapLEPm3R3EOrlQiM8cRLKq3Rt02ZVVwT0SkZstSg";
            string deploymentId = "0a18471f93d448e2a1f60e47e03d3413";
            string grantType = "external_auth";
            string externalAuthType = "deviceid_access_token";
            string externalAuthToken = await EOS.GetExternalAuthTokenAsync(clientId, clientSecret, externalAuthType);
            string accessToken = await EOS.GetAccessTokenAsync(clientId, clientSecret, deploymentId, grantType, externalAuthType, externalAuthToken);

            SaveResult(nameof(GetMatchmakingAsyncTest), await EOS.GetMatchmakingAsync(deploymentId, accessToken));
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            // Ark: Survival Ascended
            string clientId = "xyza7891muomRmynIIHaJB9COBKkwj6n";
            string clientSecret = "PP5UGxysEieNfSrEicaD1N2Bb3TdXuD7xHYcsdUHZ7s";
            string deploymentId = "ad9a8feffb3b4b2ca315546f038c3ae2";
            string grantType = "client_credentials";
            string externalAuthType = "";
            string externalAuthToken = "";
            string accessToken = await EOS.GetAccessTokenAsync(clientId, clientSecret, deploymentId, grantType, externalAuthType, externalAuthToken);

            EOS eos = new("5.62.115.46", 7783, deploymentId, accessToken, 5000);

            SaveResult(nameof(GetInfoTest), await eos.GetInfo());
        }

        [TestMethod()]
        public async Task GetInfoPalWorldTest()
        {
            // Palworld
            string clientId = "xyza78916PZ5DF0fAahu4tnrKKyFpqRE";
            string clientSecret = "j0NapLEPm3R3EOrlQiM8cRLKq3Rt02ZVVwT0SkZstSg";
            string deploymentId = "0a18471f93d448e2a1f60e47e03d3413";
            string grantType = "external_auth";
            string externalAuthType = "deviceid_access_token";
            string externalAuthToken = await EOS.GetExternalAuthTokenAsync(clientId, clientSecret, externalAuthType);
            string accessToken = await EOS.GetAccessTokenAsync(clientId, clientSecret, deploymentId, grantType, externalAuthType, externalAuthToken);

            EOS eos = new("34.22.135.67", 30001, deploymentId, accessToken, 5000);

            SaveResult(nameof(GetInfoTest), await eos.GetInfo());
        }
    }
}
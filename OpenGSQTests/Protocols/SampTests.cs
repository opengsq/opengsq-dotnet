using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SampTests : TestBase
    {
        // Samp
        public Samp samp = new("51.254.178.238", 7777);

        public SampTests() : base(nameof(SampTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await samp.GetStatus());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await samp.GetPlayers());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await samp.GetRules());
        }
    }
}
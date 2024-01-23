using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class WONTests : TestBase
    {
        public WON won = new("212.227.190.150", 27020);

        public WONTests() : base(typeof(WONTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await won.GetInfo());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await won.GetPlayers());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await won.GetRules());
        }
    }
}
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class KillingFloorTests : TestBase
    {
        public KillingFloor killingFloor = new("185.80.128.168", 7708);

        public KillingFloorTests() : base(nameof(KillingFloorTests))
        {
            EnableSave = true;
        }

        [TestMethod()]
        public async Task GetDetailsTest()
        {
            SaveResult(nameof(GetDetailsTest), await killingFloor.GetDetails());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await killingFloor.GetRules());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await killingFloor.GetPlayers());
        }
    }
}
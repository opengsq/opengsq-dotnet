using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class BattlefieldTests : TestBase
    {
        // mtasa
        public Battlefield battlefield = new("94.250.199.214", 47200, 10000);

        public BattlefieldTests() : base(nameof(BattlefieldTests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await battlefield.GetInfo());
        }

        [TestMethod()]
        public async Task GetVersionTest()
        {
            SaveResult(nameof(GetVersionTest), await battlefield.GetVersion());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await battlefield.GetPlayers());
        }
    }
}
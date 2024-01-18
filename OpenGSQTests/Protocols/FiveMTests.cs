using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class FiveMTests : TestBase
    {
        // fivem
        public FiveM fivem = new("144.217.10.12", 30120);

        public FiveMTests() : base(nameof(FiveMTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await fivem.GetInfo());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await fivem.GetPlayers());
        }

        [TestMethod()]
        public async Task GetDynamicTest()
        {
            SaveResult(nameof(GetDynamicTest), await fivem.GetDynamic());
        }
    }
}
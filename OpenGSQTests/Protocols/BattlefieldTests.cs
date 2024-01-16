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
        public void GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), battlefield.GetInfo());
        }

        [TestMethod()]
        public void GetVersionTest()
        {
            SaveResult(nameof(GetVersionTest), battlefield.GetVersion());
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), battlefield.GetPlayers());
        }
    }
}
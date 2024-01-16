using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class VcmpTests : TestBase
    {
        // Vcmp
        public Vcmp vcmp = new("51.178.65.136", 8114);

        public VcmpTests() : base(nameof(VcmpTests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), vcmp.GetStatus());
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), vcmp.GetPlayers());
        }
    }
}
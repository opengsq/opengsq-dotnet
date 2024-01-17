using System.Threading.Tasks;
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
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await vcmp.GetStatus());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await vcmp.GetPlayers());
        }
    }
}
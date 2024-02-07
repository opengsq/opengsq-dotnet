using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SourceTests : TestBase
    {
        // TF2
        public Source source = new("45.62.160.71", 27015);

        // Compressed response
        // public Source source = new("146.19.87.161", 27015);

        // The Ship
        // public Source source = new Source("5.79.86.193", 27021);

        public SourceTests() : base(typeof(SourceTests))
        {
            // EnableSave = true;
            DelayPerTest = 1000;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await source.GetInfo());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await source.GetPlayers());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await source.GetRules());
        }
    }
}
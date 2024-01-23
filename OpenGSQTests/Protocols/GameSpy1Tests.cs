using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy1Tests : TestBase
    {
        // Battlefield Vietnam (Old Response)
        public GameSpy1 gameSpy1 = new("139.162.235.20", 7778);

        // Battlefield Vietnam (XServerQuery)
        //public GameSpy1 gameSpy1 = new GameSpy1("66.150.121.123", 7778);

        public GameSpy1Tests() : base(typeof(GameSpy1Tests))
        {
            // EnableSave = true;
            DelayPerTest = 900;
        }

        [TestMethod()]
        public async Task GetBasicTest()
        {
            SaveResult(nameof(GetBasicTest), await gameSpy1.GetBasic());
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await gameSpy1.GetInfo());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await gameSpy1.GetRules());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await gameSpy1.GetPlayers());
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await gameSpy1.GetStatus());
        }

        [TestMethod()]
        public async Task GetTeamsTest()
        {
            SaveResult(nameof(GetTeamsTest), await gameSpy1.GetTeams());
        }

        [TestMethod()]
        public async Task GetEchoTest()
        {
            SaveResult(nameof(GetEchoTest), await gameSpy1.GetEcho());
        }
    }
}
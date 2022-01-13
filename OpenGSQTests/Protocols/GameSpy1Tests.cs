using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy1Tests : TestBase
    {
        // Battlefield Vietnam (Old Response)
        public GameSpy1 gameSpy1 = new GameSpy1("139.162.235.20", 7778);

        // Battlefield Vietnam (XServerQuery)
        //public GameSpy1 gameSpy1 = new GameSpy1("66.150.121.123", 7778);

        public GameSpy1Tests() : base(nameof(GameSpy1Tests))
        {
            _EnableSave = false;
            _DelayPerTest = 900;
        }

        [TestMethod()]
        public void GetBasicTest()
        {
            SaveResult(nameof(GetBasicTest), gameSpy1.GetBasic());
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), gameSpy1.GetInfo());
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), gameSpy1.GetRules());
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), gameSpy1.GetPlayers());
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), gameSpy1.GetStatus());
        }

        [TestMethod()]
        public void GetTeamsTest()
        {
            SaveResult(nameof(GetTeamsTest), gameSpy1.GetTeams());
        }

        [TestMethod()]
        public void GetEchoTest()
        {
            SaveResult(nameof(GetEchoTest), gameSpy1.GetEcho());
        }
    }
}
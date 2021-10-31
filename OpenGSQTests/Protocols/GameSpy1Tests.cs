using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Collections.Generic;
using System.Text.Json;

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
            var response = gameSpy1.GetBasic();

            SaveResult(nameof(GetBasicTest), JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), Options));
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            var response = gameSpy1.GetInfo();

            SaveResult(nameof(GetInfoTest), JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), Options));
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            var response = gameSpy1.GetRules();

            SaveResult(nameof(GetRulesTest), JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), Options));
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            var response = gameSpy1.GetPlayers();

            SaveResult(nameof(GetPlayersTest), JsonSerializer.Serialize(response, typeof(List<Dictionary<string, string>>), Options));
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = gameSpy1.GetStatus();

            SaveResult(nameof(GetStatusTest), JsonSerializer.Serialize(response, typeof(GameSpy1.Status), Options));
        }

        [TestMethod()]
        public void GetTeamsTest()
        {
            var response = gameSpy1.GetTeams();

            SaveResult(nameof(GetTeamsTest), JsonSerializer.Serialize(response, typeof(List<Dictionary<string, string>>), Options));
        }

        [TestMethod()]
        public void GetEchoTest()
        {
            var response = gameSpy1.GetEcho();

            SaveResult(nameof(GetEchoTest), JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), Options));
        }
    }
}
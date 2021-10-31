using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy3Tests : TestBase
    {
        // Battlefield 2
        public GameSpy3 gameSpy3 = new GameSpy3("185.107.96.59", 29900);

        public GameSpy3Tests() : base(nameof(GameSpy3Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = gameSpy3.GetStatus();

            SaveResult(nameof(GetStatusTest), JsonSerializer.Serialize(response, typeof(GameSpy3.Status), Options));
        }
    }
}
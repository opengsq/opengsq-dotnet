using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy2Tests : TestBase
    {
        // Battlefield Vietnam
        public GameSpy2 gameSpy2 = new GameSpy2("158.69.118.94", 23000);

        public GameSpy2Tests() : base(nameof(GameSpy2Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = gameSpy2.GetStatus();

            SaveResult(nameof(GetStatusTest), JsonSerializer.Serialize(response, typeof(GameSpy2.Status), Options));
        }
    }
}
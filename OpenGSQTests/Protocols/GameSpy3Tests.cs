using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy3Tests : TestBase
    {
        // Battlefield 2
        public GameSpy3 gameSpy3 = new GameSpy3("95.172.92.116", 29900);

        public GameSpy3Tests() : base(nameof(GameSpy3Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), gameSpy3.GetStatus());
        }
    }
}
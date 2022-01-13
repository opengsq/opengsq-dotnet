using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy4Tests : TestBase
    {
        // Minecraft Pocket Edition
        public GameSpy4 gameSpy4 = new GameSpy4("104.238.152.181", 19132);

        public GameSpy4Tests() : base(nameof(GameSpy4Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), gameSpy4.GetStatus());
        }
    }
}
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy4Tests : TestBase
    {
        // Minecraft Pocket Edition
        public GameSpy4 gameSpy4 = new("play.avengetech.me", 19132);

        public GameSpy4Tests() : base(typeof(GameSpy4Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await gameSpy4.GetStatus());
        }
    }
}
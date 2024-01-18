using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy3Tests : TestBase
    {
        // Battlefield 2
        public GameSpy3 gameSpy3 = new("95.172.92.116", 29900);

        public GameSpy3Tests() : base(nameof(GameSpy3Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await gameSpy3.GetStatus());
        }
    }
}
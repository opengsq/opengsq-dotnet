using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy2Tests : TestBase
    {
        // Battlefield Vietnam
        public GameSpy2 gameSpy2 = new("108.61.236.22", 23000);

        public GameSpy2Tests() : base(typeof(GameSpy2Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await gameSpy2.GetStatus());
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

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
            SaveResult(nameof(GetStatusTest), gameSpy2.GetStatus());
        }
    }
}
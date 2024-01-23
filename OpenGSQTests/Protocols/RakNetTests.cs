using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class RakNetTests : TestBase
    {
        // minecraftpe
        public RakNet raknet = new("mc.advancius.net", 19132);

        public RakNetTests() : base(typeof(RakNetTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await raknet.GetStatus());
        }
    }
}
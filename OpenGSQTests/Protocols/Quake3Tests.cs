using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake3Tests : TestBase
    {
        // Quake3 - https://www.gametracker.com/search/et/
        public Quake3 quake3 = new("108.61.18.110", 27960);

        public Quake3Tests() : base(typeof(Quake3Tests))
        {
            // EnableSave = true;
            DelayPerTest = 900;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await quake3.GetInfo());
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await quake3.GetStatus());
        }
    }
}
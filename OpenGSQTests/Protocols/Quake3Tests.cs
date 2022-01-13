using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake3Tests : TestBase
    {
        public Quake3Tests() : base(nameof(Quake3Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            // Quake3 - https://www.gametracker.com/search/et/
            var quake3 = new Quake3("108.61.18.110", 27960);

            SaveResult(nameof(GetInfoTest), quake3.GetInfo());
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var quake3 = new Quake3("108.61.21.93", 27960);

            SaveResult(nameof(GetStatusTest), quake3.GetStatus());
        }
    }
}
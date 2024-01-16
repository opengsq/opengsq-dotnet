using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake3Tests : TestBase
    {
        // Quake3 - https://www.gametracker.com/search/et/
        public Quake3 quake3 = new("108.61.18.110", 27960);

        public Quake3Tests() : base(nameof(Quake3Tests))
        {
            _EnableSave = false;
            _DelayPerTest = 900;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), quake3.GetInfo());
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), quake3.GetStatus());
        }
    }
}
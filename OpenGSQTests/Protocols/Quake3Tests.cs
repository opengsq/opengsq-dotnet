using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake3Tests : TestBase
    {
        // Quake3 - https://www.gametracker.com/search/et/

        public Quake3Tests() : base(nameof(Quake3Tests))
        {
            _EnableSave = false;
            _DelayPerTest = 900;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            Quake3 quake3 = new Quake3("108.61.18.109", 27920);
            SaveResult(nameof(GetInfoTest), quake3.GetInfo());
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            Quake3 quake3 = new Quake3("108.61.18.110", 27960);
            SaveResult(nameof(GetStatusTest), quake3.GetStatus());
        }
    }
}
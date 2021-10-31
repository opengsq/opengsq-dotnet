using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Collections.Generic;
using System.Text.Json;

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
            var response = quake3.GetInfo();

            SaveResult(nameof(GetInfoTest), JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), Options));
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var quake3 = new Quake3("108.61.21.93", 27960);
            var response = quake3.GetStatus();

            SaveResult(nameof(GetStatusTest), JsonSerializer.Serialize(response, typeof(Quake3.Status), Options));
        }
    }
}
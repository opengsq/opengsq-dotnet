using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake2Tests : TestBase
    {
        // Quake2
        public Quake2 quake2 = new Quake2("46.165.236.118", 27910);

        public Quake2Tests() : base(nameof(Quake2Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = quake2.GetStatus();

            SaveResult(nameof(GetStatusTest), JsonSerializer.Serialize(response, typeof(Quake2.Status), Options));
        }
    }
}
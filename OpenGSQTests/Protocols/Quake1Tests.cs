using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake1Tests : TestBase
    {
        // QuakeWorld
        public Quake1 quake1 = new("35.185.44.174", 27500);

        public Quake1Tests() : base(typeof(Quake1Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await quake1.GetStatus());
        }
    }
}
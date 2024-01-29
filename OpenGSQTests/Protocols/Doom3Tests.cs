using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Doom3Tests : TestBase
    {
        // doom3
        // public Doom3 doom3 = new("66.85.14.240", 27666);

        // etqw
        public Doom3 doom3 = new("178.162.135.83", 27735);

        public Doom3Tests() : base(typeof(Doom3Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await doom3.GetStatus());
        }
    }
}
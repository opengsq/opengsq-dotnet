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

        public Doom3Tests() : base(nameof(Doom3Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), doom3.GetInfo());
        }
    }
}
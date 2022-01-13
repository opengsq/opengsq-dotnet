using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake1Tests : TestBase
    {
        // QuakeWorld
        public Quake1 quake1 = new Quake1("35.185.44.174", 27500);

        public Quake1Tests() : base(nameof(Quake1Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), quake1.GetStatus());
        }
    }
}
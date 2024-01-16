using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class ASETests : TestBase
    {
        // mtasa
        public ASE ase = new("79.137.97.3", 22126);

        public ASETests() : base(nameof(ASETests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), ase.GetStatus());
        }
    }
}
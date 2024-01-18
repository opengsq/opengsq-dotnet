using System.Threading.Tasks;
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
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await ase.GetStatus());
        }
    }
}
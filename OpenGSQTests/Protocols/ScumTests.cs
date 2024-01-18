using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class ScumTests : TestBase
    {
        public Scum scum = new("15.235.181.19", 7042);

        public ScumTests() : base(nameof(ScumTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await scum.GetStatus());
        }
    }
}
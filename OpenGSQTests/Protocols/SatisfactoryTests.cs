using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SatisfactoryTests : TestBase
    {
        public Satisfactory satisfactory = new("79.136.0.124", 15777);

        public SatisfactoryTests() : base(nameof(SatisfactoryTests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await satisfactory.GetStatus());
        }
    }
}
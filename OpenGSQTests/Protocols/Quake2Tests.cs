using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake2Tests : TestBase
    {
        // Quake2
        public Quake2 quake2 = new("46.165.236.118", 27910);

        public Quake2Tests() : base(nameof(Quake2Tests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public async Task GetStatusTest()
        {
            SaveResult(nameof(GetStatusTest), await quake2.GetStatus());
        }
    }
}
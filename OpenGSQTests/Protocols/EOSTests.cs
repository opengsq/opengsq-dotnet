using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class EOSTests : TestBase
    {
        private readonly static string clientId = "xyza7891muomRmynIIHaJB9COBKkwj6n";
        private readonly static string clientSecret = "PP5UGxysEieNfSrEicaD1N2Bb3TdXuD7xHYcsdUHZ7s";
        private readonly static string deploymentId = "ad9a8feffb3b4b2ca315546f038c3ae2";

        // arksa
        public EOS eos = new("5.62.115.46", 7783, 5000, clientId, clientSecret, deploymentId);

        public EOSTests() : base(nameof(EOSTests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await eos.GetInfo());
        }
    }
}
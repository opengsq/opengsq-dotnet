using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Teamspeak3Tests : TestBase
    {
        public Teamspeak3 teamspeak3 = new("145.239.200.2", 10011, 9987);

        public Teamspeak3Tests() : base(nameof(Teamspeak3Tests))
        {
            _EnableSave = !false;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await teamspeak3.GetInfo());
        }

        [TestMethod()]
        public async Task GetClientsTest()
        {
            SaveResult(nameof(GetClientsTest), await teamspeak3.GetClients());
        }

        [TestMethod()]
        public async Task GetChannelsTest()
        {
            SaveResult(nameof(GetChannelsTest), await teamspeak3.GetChannels());
        }
    }
}
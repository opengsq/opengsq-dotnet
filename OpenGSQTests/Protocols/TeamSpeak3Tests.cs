using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class TeamSpeak3Tests : TestBase
    {
        public TeamSpeak3 teamSpeak3 = new("145.239.200.2", 10011, 9987);

        public TeamSpeak3Tests() : base(nameof(TeamSpeak3Tests))
        {
            // EnableSave = true;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await teamSpeak3.GetInfo());
        }

        [TestMethod()]
        public async Task GetClientsTest()
        {
            SaveResult(nameof(GetClientsTest), await teamSpeak3.GetClients());
        }

        [TestMethod()]
        public async Task GetChannelsTest()
        {
            SaveResult(nameof(GetChannelsTest), await teamSpeak3.GetChannels());
        }
    }
}
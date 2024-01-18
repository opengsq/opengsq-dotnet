using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System;
using System.Threading.Tasks;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SourceTests : TestBase
    {
        // TF2
        public Source source = new("45.62.160.71", 27015);

        // The Ship
        // public Source source = new Source("5.79.86.193", 27021);

        public SourceTests() : base(nameof(SourceTests))
        {
            // EnableSave = true;
            DelayPerTest = 1000;
        }

        [TestMethod()]
        public async Task GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), await source.GetInfo());
        }

        [TestMethod()]
        public async Task GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), await source.GetPlayers());
        }

        [TestMethod()]
        public async Task GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), await source.GetRules());
        }

        [TestMethod()]
        public async Task RemoteConsoleTest()
        {
            using var remoteConsole = new Source.RemoteConsole("122.128.109.245", 27010);

            try
            {
                await remoteConsole.Authenticate("n97h79b86g68");

                string result = await remoteConsole.SendCommand("cvarlist");

                SaveResult(nameof(RemoteConsoleTest), result, isJson: false);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"{e.Message}");
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }
    }
}
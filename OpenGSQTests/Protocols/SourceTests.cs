using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System;

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
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            SaveResult(nameof(GetInfoTest), source.GetInfo());
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            SaveResult(nameof(GetPlayersTest), source.GetPlayers());
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            SaveResult(nameof(GetRulesTest), source.GetRules());
        }

        [TestMethod()]
        public void RemoteConsoleTest()
        {
            using var remoteConsole = new Source.RemoteConsole("", 27010);

            try
            {
                remoteConsole.Authenticate("");

                string result = remoteConsole.SendCommand("cvarlist");

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
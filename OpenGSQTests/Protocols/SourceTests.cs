using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenGSQTests;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SourceTests : TestBase
    {
        // TF2
        public Source source = new Source("91.216.250.14", 27015);

        // The Ship
        // public A2S a2s = new A2S("5.79.86.193", 27021);

        public SourceTests() : base(nameof(SourceTests))
        {
            _EnableSave = false;
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            var (info, infoType) = source.GetInfo();

            SaveResult(nameof(GetInfoTest), JsonSerializer.Serialize(info, infoType, Options));
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            var players = source.GetPlayers();

            SaveResult(nameof(GetPlayersTest), JsonSerializer.Serialize(players, typeof(List<Source.Player>), Options));
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            var rules = source.GetRules();

            SaveResult(nameof(GetRulesTest), JsonSerializer.Serialize(rules, typeof(Dictionary<string, string>), Options));
        }

        [TestMethod()]
        public void RemoteConsoleTest()
        {
            var rcon = new Source.RemoteConsole("", 27010);

            try
            {
                rcon.Authenticate("");

                string response = rcon.SendCommand("cvarlist");

                SaveResult(nameof(RemoteConsoleTest), response, isJson: false);
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
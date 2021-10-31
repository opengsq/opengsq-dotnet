using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class SourceTests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // TF2
        public Source source = new Source("91.216.250.14", 27015);

        // The Ship
        // public A2S a2s = new A2S("5.79.86.193", 27021);

        [TestMethod()]
        public void GetInfoTest()
        {
            var (info, infoType) = source.GetInfo();

            Console.WriteLine("Info Type:\t" + (infoType.Equals(typeof(Source.Info.Source)) ? "Source" : "GoldSource"));
            Console.WriteLine();
            Console.WriteLine(JsonSerializer.Serialize(info, infoType, options));
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            var players = source.GetPlayers();

            Console.WriteLine("Player Count:\t" + players.Count);
            Console.WriteLine();
            Console.WriteLine(JsonSerializer.Serialize(players, typeof(List<Source.Player>), options));
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            var rules = source.GetRules();

            Console.WriteLine("Rule Count:\t" + rules.Count);
            Console.WriteLine();
            Console.WriteLine(JsonSerializer.Serialize(rules, typeof(Dictionary<string, string>), options));
        }

        [TestMethod()]
        public void RemoteConsoleTest()
        {
            var rcon = new Source.RemoteConsole("", 27015);

            try
            {
                rcon.Authenticate("");
                string response = rcon.SendCommand("cvarlist");
                Console.WriteLine(response);
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
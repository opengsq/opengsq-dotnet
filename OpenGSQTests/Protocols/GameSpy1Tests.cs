using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy1Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Battlefield Vietnam (Old Response)
        public GameSpy1 gameSpy1 = new GameSpy1("139.162.235.20", 7778);

        // Battlefield Vietnam (XServerQuery)
        //public GameSpy1 gameSpy1 = new GameSpy1("66.150.121.123", 7778);

        // Add a delay on every test case since too quick causes timeout
        private readonly int _millisecondsTimeout = 900;

        [TestMethod()]
        public void GetBasicTest()
        {
            var response = gameSpy1.GetBasic();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetInfoTest()
        {
            var response = gameSpy1.GetInfo();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetRulesTest()
        {
            var response = gameSpy1.GetRules();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetPlayersTest()
        {
            var response = gameSpy1.GetPlayers();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(List<Dictionary<string, string>>), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = gameSpy1.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(GameSpy1.Status), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetTeamsTest()
        {
            var response = gameSpy1.GetTeams();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(List<Dictionary<string, string>>), options));

            Thread.Sleep(_millisecondsTimeout);
        }

        [TestMethod()]
        public void GetEchoTest()
        {
            var response = gameSpy1.GetEcho();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), options));

            Thread.Sleep(_millisecondsTimeout);
        }
    }
}
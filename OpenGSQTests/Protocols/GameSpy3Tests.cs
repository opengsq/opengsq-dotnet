using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy3Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Battlefield 2
        public GameSpy3 gameSpy3 = new GameSpy3("185.107.96.59", 29900);

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = gameSpy3.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(GameSpy3.Status), options));
        }
    }
}
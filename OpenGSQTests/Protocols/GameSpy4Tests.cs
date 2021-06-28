using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy4Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Minecraft
        public GameSpy4 gameSpy4 = new GameSpy4("54.39.131.161", 25565);

        [TestMethod()]
        public void GetResponseTest()
        {
            var response = gameSpy4.GetResponse();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(GameSpy3.Response), options));
        }
    }
}
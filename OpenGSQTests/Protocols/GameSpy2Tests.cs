using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class GameSpy2Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Battlefield Vietnam
        public GameSpy2 gameSpy2 = new GameSpy2("158.69.118.94", 23000);

        [TestMethod()]
        public void GetResponseTest()
        {
            var response = gameSpy2.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(GameSpy2.Status), options));
        }
    }
}
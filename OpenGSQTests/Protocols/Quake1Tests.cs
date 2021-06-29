using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake1Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // QuakeWorld
        public Quake1 quake1 = new Quake1("35.185.44.174", 27500);

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = quake1.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Quake1.Status), options));
        }
    }
}
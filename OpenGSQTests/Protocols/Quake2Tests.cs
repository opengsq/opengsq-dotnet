using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake2Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Quake2
        public Quake2 quake2 = new Quake2("46.165.236.118", 27910);

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = quake2.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Quake2.Status), options));
        }
    }
}
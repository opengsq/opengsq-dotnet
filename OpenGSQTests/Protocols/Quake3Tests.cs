using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenGSQ.Protocols.Tests
{
    [TestClass()]
    public class Quake3Tests
    {
        public JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        // Quake3 - https://www.gametracker.com/search/et/
        public Quake3 quake3 = new Quake3("108.61.18.110", 27960);

        [TestMethod()]
        public void GetInfoTest()
        {
            var response = quake3.GetInfo();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Dictionary<string, string>), options));
        }

        [TestMethod()]
        public void GetStatusTest()
        {
            var response = quake3.GetStatus();

            Console.WriteLine(JsonSerializer.Serialize(response, typeof(Quake3.Status), options));
        }
    }
}
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace OpenGSQTests
{
    public abstract partial class TestBase
    {
        /// <summary>
        /// Set save the result to file
        /// </summary>
        protected bool EnableSave = false;

        /// <summary>
        /// Delay on every test case since too quick may causes timeout
        /// </summary>
        protected int DelayPerTest = 0;

        /// <summary>
        /// OpenGSQTests Results Path
        /// </summary>
        public string ResultsPath = Path.Combine(MyRegex().Match(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Value, "Results");

        /// <summary>
        /// Protocol result path
        /// </summary>
        private readonly string _protocolPath;

        /// <summary>
        /// Json serializer options
        /// </summary>
        public JsonSerializerOptions Options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        /// <summary>
        /// TestBase
        /// </summary>
        /// <param name="protocolName"></param>
        public TestBase(string protocolName)
        {
            _protocolPath = Path.Combine(ResultsPath, protocolName);
            Directory.CreateDirectory(_protocolPath);
        }

        /// <summary>
        /// Save and print the result
        /// </summary>
        /// <param name="protocolName"></param>
        /// <param name="functionName"></param>
        /// <param name="result"></param>
        /// <param name="isJson"></param>
        public void SaveResult(string functionName, object result, bool isJson = true)
        {
            if (isJson)
            {
                result = JsonSerializer.Serialize(result, result.GetType(), Options);
            }

            if (EnableSave)
            {
                File.WriteAllText(Path.Combine(_protocolPath, $"{functionName}.{(isJson ? "json" : "txt")}"), result.ToString());
            }

            Console.WriteLine(result.ToString());

            Thread.Sleep(DelayPerTest);
        }

        [GeneratedRegex(".+OpenGSQTests")]
        private static partial Regex MyRegex();
    }
}

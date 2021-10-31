using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace OpenGSQTests
{
    public abstract class TestBase
    {
        /// <summary>
        /// Set save the result to file
        /// </summary>
        protected bool _EnableSave;

        /// <summary>
        /// Delay on every test case since too quick may causes timeout
        /// </summary>
        protected int _DelayPerTest = 0;

        /// <summary>
        /// Protocol result path
        /// </summary>
        protected string _ProtocolPath;

        /// <summary>
        /// OpenGSQTests Results Path
        /// </summary>
        public string ResultsPath = Path.Combine(Regex.Match(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".+OpenGSQTests").Value, "Results");

        /// <summary>
        /// Json serializer options
        /// </summary>
        public JsonSerializerOptions Options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        /// <summary>
        /// TestBase
        /// </summary>
        /// <param name="protocolName"></param>
        public TestBase(string protocolName)
        {
            _ProtocolPath = Path.Combine(ResultsPath, protocolName);
            Directory.CreateDirectory(_ProtocolPath);
        }

        /// <summary>
        /// Save and print the result
        /// </summary>
        /// <param name="protocolName"></param>
        /// <param name="functionName"></param>
        /// <param name="result"></param>
        /// <param name="isJson"></param>
        public void SaveResult(string functionName, string result, bool isJson = true)
        {
            if (_EnableSave && !string.IsNullOrWhiteSpace(result))
            {
                File.WriteAllText(Path.Combine(_ProtocolPath, $"{functionName}.{(isJson ? "json" : "txt")}"), result);
            }

            Console.WriteLine(result);

            Thread.Sleep(_DelayPerTest);
        }
    }
}

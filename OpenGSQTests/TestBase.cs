using System.IO;
using System.Linq;
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
        /// The full path and filename of the file.
        /// </summary>
        private static readonly string _filePath = MyRegex().Match(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Value;

        /// <summary>
        /// OpenGSQTests Results Base Path
        /// Path: /OpenGSQTests/Results
        /// </summary>
        public string ResultsBasePath = Path.Combine(_filePath, "Results");

        /// <summary>
        /// OpenGSQTests Docs Base Path
        /// Path: /docs/tests/
        /// </summary>
        public string DocsBasePath = Path.GetFullPath(Path.Combine(_filePath, "..", "docs", "tests"));

        /// <summary>
        /// Protocol Name
        /// </summary>
        private readonly string _protocolName;

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
            _protocolName = protocolName;

            UpdateDocsTocYML();
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
                // Write the result to Results folder
                string resultsPath = Path.Combine(ResultsBasePath, _protocolName);
                Directory.CreateDirectory(resultsPath);
                File.WriteAllText(Path.Combine(resultsPath, $"{functionName}.{(isJson ? "json" : "txt")}"), result.ToString());

                string docsPath = Path.Combine(DocsBasePath, _protocolName);
                Directory.CreateDirectory(docsPath);

                // Generate /docs/tests/{_protocolName}/{functionName}.md
                string contents = $"---\nuid: {nameof(OpenGSQ)}.Protocols.Tests.{_protocolName}.{functionName}\n---\n\n";
                contents += $"# Test Method {functionName}\n\nHere are the results for the test method.\n\n";
                contents += $"```{(isJson ? "json" : "txt")}\n{result}\n```\n";
                File.WriteAllText(Path.Combine(docsPath, $"{functionName}.md"), contents);
            }

            Thread.Sleep(DelayPerTest);
        }

        public void UpdateDocsTocYML()
        {
            var baseType = typeof(TestBase);
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToList();

            // Generate Table Of Content for /docs/tests/
            string contents = "### YamlMime:TableOfContent\nitems:\n";

            foreach (var type in types)
            {
                contents += $"- uid: {type.FullName}\n  name: {type.Name}\n  items:\n";

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var method in methods)
                {
                    contents += $"  - uid: {type.FullName}.{method.Name}\n    name: {method.Name}\n";
                }

                // Generate /docs/tests/{_protocolName}/{_protocolName}.md
                if (_protocolName == type.Name)
                {
                    CreateTestClassMdFile(methods);
                }
            }

            File.WriteAllText(Path.Combine(DocsBasePath, "toc.yml"), contents);
        }

        public void CreateTestClassMdFile(MethodInfo[] methods)
        {
            string docsPath = Path.Combine(DocsBasePath, _protocolName);
            Directory.CreateDirectory(docsPath);

            string contents2 = $"---\nuid: {nameof(OpenGSQ)}.Protocols.Tests.{_protocolName}\n---\n\n";
            contents2 += $"# Test Class {_protocolName}\n\n### Test Methods\n\n";

            foreach (var method in methods)
            {
                contents2 +=  $"<a href=\"/tests/{_protocolName}/{method.Name}.html\">{method.Name}</a>\n\n";
            }

            File.WriteAllText(Path.Combine(docsPath, $"{_protocolName}.md"), contents2);
        }

        [GeneratedRegex(".+OpenGSQTests")]
        private static partial Regex MyRegex();
    }
}

using System;
using System.Collections.Generic;
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
        /// Protocol Type
        /// </summary>
        private readonly Type _type;

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
        /// <param name="type"></param>
        public TestBase(Type type)
        {
            _type = type;

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
                /* Old Method */
                // Write the result to Results folder
                // string resultsPath = Path.Combine(ResultsBasePath, _protocolName);
                // Directory.CreateDirectory(resultsPath);
                // File.WriteAllText(Path.Combine(resultsPath, $"{functionName}.{(isJson ? "json" : "txt")}"), result.ToString());

                string docsPath = Path.Combine(DocsBasePath, _type.Namespace, _type.Name);
                Directory.CreateDirectory(docsPath);

                // Generate /docs/tests/{_type.FullName}/{functionName}/{functionName}.md
                string contents = $"---\nuid: {_type.FullName}.{functionName}\n---\n\n";
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
            var _types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToList();

            SortedDictionary<string, List<Type>> items = [];

            foreach (var type in _types)
            {
                if (!items.ContainsKey(type.Namespace))
                {
                    items[type.Namespace] = [];
                }

                items[type.Namespace].Add(type);
            }

            string contents = "### YamlMime:TableOfContent\nitems:\n";

            foreach ((var nameSpace, var types) in items)
            {
                contents += $"- uid: {nameSpace}\n  name: {nameSpace}\n  items:\n";

                CreateNamespaceMdFile(nameSpace, types);

                foreach (var type in types)
                {
                    contents += $"  - uid: {type.FullName}\n    name: {type.Name}\n    items:\n";

                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    foreach (var method in methods)
                    {
                        contents += $"    - uid: {type.FullName}.{method.Name}\n      name: {method.Name}\n";
                    }

                    // Generate /docs/tests/{_protocolName}/{_protocolName}.md
                    if (_type.Name == type.Name)
                    {
                        CreateTestClassMdFile(type, methods);
                    }
                }
            }

            contents += "memberLayout: SamePage\n";

            File.WriteAllText(Path.Combine(DocsBasePath, "toc.yml"), contents);
        }

        public void CreateNamespaceMdFile(string nameSpace, List<Type> types)
        {
            string docsPath = Path.Combine(DocsBasePath, nameSpace);
            Directory.CreateDirectory(docsPath);

            string contents = $"---\nuid: {nameSpace}\n---\n";
            contents += $"\n# Namespace {nameSpace}\n\n### Classes\n";

            foreach (var type in types)
            {
                contents += $"\n[{type.Name}](xref:{type.FullName})\n";
            }

            File.WriteAllText(Path.Combine(docsPath, $"{nameSpace}.md"), contents);
        }

        public void CreateTestClassMdFile(Type type, MethodInfo[] methods)
        {
            string docsPath = Path.Combine(DocsBasePath, type.Namespace, type.Name);
            Directory.CreateDirectory(docsPath);

            string contents = $"---\nuid: {type.Namespace}.{type.Name}\n---\n";
            contents += $"\n# Test Class {type.FullName}\n\n### Test Methods\n";

            foreach (var method in methods)
            {
                contents += $"\n[{method.Name}](xref:{type.FullName}.{method.Name})\n";
            }

            File.WriteAllText(Path.Combine(docsPath, $"{type.Name}.md"), contents);
        }

        [GeneratedRegex(".+OpenGSQTests")]
        private static partial Regex MyRegex();
    }
}

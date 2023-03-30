using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Pyro.Test {
    public class TestCommonBase
        : Process {
        private const string ScriptsFolder = "Scripts";
        private const string OutputFolder = "Output";

        protected static void WriteLine(string fmt, params object[] args) {
            var text = fmt;
            if (args != null && args.Length > 0) {
                text = string.Format(fmt, args);
            }

            DebugTraceLine(text);
        }

        protected static void DebugTraceLine(string text) {
            TestContext.Out.WriteLine(text);
            Trace.WriteLine(text);
            Console.WriteLine(text);
        }

        protected static string GetScriptsPath() {
            return MakeLocalPath(ScriptsFolder);
        }

        protected static string GetOutputPath() {
            return MakeLocalPath(OutputFolder);
        }

        private static string MakeLocalPath(string relative) {
            return Path.Combine(GetFolderRoot(), relative);
        }

        private static string GetFolderRoot() {
            var testDirectory = TestContext.CurrentContext.TestDirectory;
            return testDirectory.Replace(@"\bin\Debug", "").Replace(@"netcoreapp3.0", "");
        }

        protected string GetFullScriptPathname(string scriptName) {
            return Path.Combine(GetScriptsPath(), scriptName);
        }

        protected string LoadScript(string fileName) {
            return File.ReadAllText(GetFullScriptPathname(fileName));
        }
    }
}
using System;
using System.IO;
using Diver.Language;
using NUnit.Compatibility;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiScripts : TestCommon
    {
        public bool Verbose = true;

        [Test]
        public void RunScripts()
        {
            var root = TestContext.CurrentContext.TestDirectory.Replace(@"\bin\Debug", "");
            var path = Path.Combine(root, ScriptsFolder);
            WriteLine($"Scripts in: {path}");
            var files = Directory.GetFiles(path, "*.pi");
            foreach (var file in files)
                RunScript(file);
        }

        private void RunScript(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            try
            {
                WriteLine($"Running {fileName}");
                var text = File.ReadAllText(filePath);
                var trans = new PiTranslator(_reg, text);
                if (Verbose)
                    WriteLine($"Translator for {filePath}: {trans}");
                if (trans.Failed)
                    WriteLine($"Error: {trans.Error}");
                Assert.IsFalse(trans.Failed);
                _exec.Continue(trans.Continuation);
            }
            catch (Exception e)
            {
                WriteLine($"Script {fileName}: Exception={e.Message}");
            }
        }
    }
}
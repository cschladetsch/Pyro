using System.IO;
using NUnit.Framework;
using Pyro.Impl;
using Pyro.Language.Tau;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;

namespace TestTau {
    [TestFixture]
    public class TestTauGenerator
        : TestTauCommon {
        [Test]
        public void TestGenerator0() {
            TestGenerator("Test0");
        }

        [Test]
        public void TestGenerator1() {
            TestGenerator("Test1");
        }

        private void TestGenerator(string baseName) {
            var lexer = new TauLexer(LoadTauScript(baseName + ".tau"));
            if (!lexer.Process()) {
                Assert.Fail("Lexer failed: {0}", lexer.Error);
            }

            var registry = new Registry();
            var parser = new TauParser(lexer, registry);
            if (!parser.Process()) {
                Assert.Fail("Parser failed: {0}", parser.Error);
            }

            var baseFolder = GetOutputPath();

            var generator = new TauGenerator(parser);
            if (!generator.Process(Path.Combine(baseFolder, baseName), true, true)) {
                Assert.Fail("Generator failed: {0}", generator.Error);
            }
        }
    }
}

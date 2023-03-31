using System.IO;

using NUnit.Framework;

using Pyro.Impl;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;
using Pyro.Test;

namespace TestTau {
    public class TestTauParser
        : TestCommon {
        [Test]
        public void TestParser0() {
            TestTauScript("Test0.tau");
        }

        [Test]
        public void TestParser1() {
            TestTauScript("Test1.tau");
        }

        private void TestTauScript(string fileName) {
            var lexer = new TauLexer(LoadTauScript(fileName));

            if (!lexer.Process()) {
                Assert.Fail("Lexer failed:\n{0}", lexer.Error);
            }

            var registry = new Registry();
            var parser = new TauParser(lexer, registry);
            if (!parser.Process()) {
                Assert.Fail("Parser failed:\n{0}", parser.Error);
            }
        }

        private string LoadTauScript(string fileName) {
            return File.ReadAllText(GetFullScriptPathname(fileName));
        }
    }
}
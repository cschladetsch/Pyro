using System.IO;
using Pyro.Impl;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;
using Pyro.Test;

using NUnit.Framework;

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

        public void TestTauScript(string fileName) {
            var lexer = new TauLexer(LoadTauScript(fileName));

            lexer.Process();
            if (lexer.Failed) {
                Assert.Fail("Lexer failed: {0}", lexer.Error);
            }

            var registry = new Registry();
            var parser = new TauParser(lexer, registry);
            parser.Process();
            if (parser.Failed) {
                Assert.Fail("Parser failed: {0}", parser.Error);
            }
        }

        private string LoadTauScript(string fileName) {
            return File.ReadAllText(GetFullScriptPathname(fileName));
        }
    }
}

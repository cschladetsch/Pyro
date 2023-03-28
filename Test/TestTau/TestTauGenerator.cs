using NUnit.Framework;
using Pyro.Impl;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;

namespace TestTau {
    [TestFixture]
    public class TestTauGenerator
        : TestTauCommon {
        [Test]
        public void TestGenerator1() {
            var lexer = new TauLexer(LoadTauScript("Test0.tau"));
            if (!lexer.Process()) {
                Assert.Fail("Lexer failed: {0}", lexer.Error);
            }
            var registry = new Registry();
            var parser = new TauParser(lexer, registry);
            if (!parser.Process()) {
                Assert.Fail("Parser failed: {0}", parser.Error);
            }
            var generator = new TauGenerator(parser);
            if (!generator.Process()) {
                Assert.Fail("Generator failed: {0}", generator.Error);
            }
        }
    }
}
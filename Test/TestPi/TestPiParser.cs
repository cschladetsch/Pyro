using NUnit.Framework;

using Diver.LanguageCommon;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new Diver.PiLang.Lexer("1 2 3");
            var parser = new Diver.PiLang.Parser(lexer);
            parser.Process(lexer, EStructure.None);

        }
    }
}

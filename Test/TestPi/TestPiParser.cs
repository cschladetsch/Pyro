using NUnit.Framework;

using Diver.Language;
using Diver.Language.PiLang;

namespace Diver.Test
{
    [TestFixture()]
    class TestPiParser : TestCommon
    {
        [Test]
        public void TestSimpleTokens()
        {
            var lexer = new Lexer("1 2 3");
            var parser = new Parser(lexer);
            parser.Process(lexer, EStructure.None);
        }
    }
}

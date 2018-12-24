using System.Linq;
using Diver.Language.PiLang;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiLexer : TestCommon
    {
        [Test]
        public void TestNumbersAndOps()
        {
            AssertSameTokens(
                "1 2 + 2 - 3 *", 
                EToken.Int, EToken.Int, EToken.Plus, 
                EToken.Int, EToken.Minus, EToken.Int, EToken.Multiply
            );
        }

        [Test]
        public void TestPathnames()
        {
            AssertSameTokens("ident", EToken.Ident);
            AssertSameTokens("ident/ident", EToken.Ident, EToken.Separator, EToken.Ident);
            AssertSameTokens("'ident/ident", EToken.Quote, EToken.Ident, EToken.Separator, EToken.Ident);
        }

        [Test]
        public void TestComments()
        {
            AssertSameTokens("// comment", EToken.Comment);
        }

        [Test]
        public void TestStrings()
        {
            AssertSameTokens("\"foo\" \"bar\" +", EToken.String, EToken.String, EToken.Plus);
        }

        private void AssertSameTokens(string input, params EToken[] tokens)
        {
            var lex = new PiLexer(input);
            if (lex.Failed)
                WriteLine("LexerFailed: {0}", lex.Error);
            Assert.IsTrue(lex.Process());
            Assert.IsTrue(lex.Tokens.Where(t => !IsWhiteSpace(t)).Select(t => t.Type).SequenceEqual(tokens));
        }

        private bool IsWhiteSpace(PiToken piToken)
        {
            switch (piToken.Type)
            {
                case EToken.Whitespace:
                case EToken.Tab:
                case EToken.NewLine:
                    return true;
            }

            return false;
        }
    }
}

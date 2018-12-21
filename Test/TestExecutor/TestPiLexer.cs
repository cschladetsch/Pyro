using System.Linq;
using NUnit.Framework;

using Diver.Language.PiLang;

namespace Diver.Test
{
    [TestFixture]
    public class TestPi
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
            var lex = new Lexer(input);
            Assert.IsTrue(lex.Process());
            Assert.IsTrue(lex.Tokens.Where(t => !IsWhiteSpace(t)).Select(t => t.Type).SequenceEqual(tokens));
        }

        private bool IsWhiteSpace(Token token)
        {
            switch (token.Type)
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

using System.Linq;
using Diver.PiLang;
using NUnit.Framework;
using Debug = System.Diagnostics.Debug;

namespace Diver.Tests
{
    [TestFixture]
    public class TestPi
    {
        [Test]
        public void TestNumbersAndOps()
        {
            AssertSameTokens(
                LexString("1 2 + 2 - 3 *"), 
                EToken.Int, EToken.Int, EToken.Plus, 
                EToken.Int, EToken.Minus, EToken.Int, EToken.Multiply
            );
        }

        [Test]
        public void TestComments()
        {
            AssertSameTokens(LexString("// comment"), EToken.Comment);
        }

        [Test]
        public void TestStrings()
        {
            AssertSameTokens(LexString("\"foo\" \"bar\" +"), EToken.String, EToken.String, EToken.Plus);
        }

        private Lexer LexString(string text, bool write = false)
        {
            var lex = new Lexer(text);
            Assert.IsTrue(lex.Process());
            if (write)
                Debug.Write(lex.ToString());
            return lex;
        }

        private void AssertSameTokens(Lexer lex, params EToken[] tokens)
        {
            var filtered = lex.Tokens.Where(t => !IsWhiteSpace(t)).Select(t => t.Type);
            Assert.IsTrue(filtered.SequenceEqual(tokens));
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

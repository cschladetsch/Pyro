using System.Collections.Generic;
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
            var lex = LexString("1 2 + 2 - 3 *");
            Compare(
                lex, 
                new []
                {
                        EToken.Int, EToken.Int, EToken.Plus, 
                        EToken.Int, EToken.Minus, EToken.Int, EToken.Multiply,
                }
            );
        }

        [Test]
        public void TestComments()
        {
            var lex = LexString("// comment");
            Compare(lex, new[] {EToken.Comment});
        }

        [Test]
        public void TestStrings()
        {
            var lex = LexString("\"foo\" \"bar\" +");
            Compare(lex, new[] {EToken.String, EToken.String, EToken.Plus});
        }

        private Lexer LexString(string text, bool write = false)
        {
            var lex = new Lexer(text);
            Assert.IsTrue(lex.Process());
            if (write)
                Debug.Write(lex.ToString());
            return lex;
        }

        private void Compare(Lexer lex, IEnumerable<EToken> tokens)
        {
            var testSequence = tokens.ToList();
            var input = lex.Tokens.ToList();
            var filtered = input.Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            Assert.IsTrue(filtered.SequenceEqual(testSequence));
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

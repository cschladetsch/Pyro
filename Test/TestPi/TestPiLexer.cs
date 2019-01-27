using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Pyro.Language.Lexer;

namespace Diver.Test
{
    [TestFixture]
    public class TestPiLexer : TestCommon
    {
        [Test]
        public void TestIdents()
        {
            var lex = PiLex("a __a a__ a_b ___");
            var tokens = lex.Tokens.Where(t => !IsWhiteSpace(t)).ToList();
            Assert.AreEqual(5, tokens.Count);
            Assert.IsTrue(tokens.Select(t => t.Text).SequenceEqual(new []{"a", "__a", "a__", "a_b", "___"}));
        }

        [Test]
        public void TestNumbersAndOps()
        {
            AssertSameTokens(
                "1 2 + 2 - 3 *", 
                EPiToken.Int, EPiToken.Int, EPiToken.Plus, 
                EPiToken.Int, EPiToken.Minus, EPiToken.Int, EPiToken.Multiply
            );
        }

        [Test]
        public void TestBreak()
        {
            AssertSameTokens("break", EPiToken.Break);
        }

        [Test]
        public void TestPathnames()
        {
            AssertSameTokens("'a", EPiToken.Quote, EPiToken.Ident);
            AssertSameTokens("a", EPiToken.Ident);
            AssertSameTokens("ident", EPiToken.Ident);
            AssertSameTokens("ident/ident", EPiToken.Ident, EPiToken.Separator, EPiToken.Ident);
            AssertSameTokens("'ident/ident", EPiToken.Quote, EPiToken.Ident, EPiToken.Separator, EPiToken.Ident);
        }

        [Test]
        public void TestComments()
        {
            AssertSameTokens("// comment", EPiToken.Comment);
        }

        [Test]
        public void TestStrings()
        {
            AssertSameTokens("\"foo\" \"bar\" +", EPiToken.String, EPiToken.String, EPiToken.Plus);
        }
    }
}

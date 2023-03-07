namespace TestTau {
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Pyro.TauLang.Lexer;

    public class Tests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            var input =
@"namespace Foo {
    class Bar {
        void fun(int n, float f, string s);
    }
}";
            AssertSameTokens(input, ETauToken.Namespace, ETauToken.Ident, ETauToken.OpenBrace, ETauToken.Class, ETauToken.Ident, ETauToken.OpenBrace, ETauToken.Void, ETauToken.Ident, ETauToken.OpenBrace, ETauToken.Int, ETauToken.Ident, ETauToken.Comma, ETauToken.Float, ETauToken.Ident, ETauToken.Comma, ETauToken.String, ETauToken.Ident, ETauToken.CloseParan, ETauToken.Semi, ETauToken.CloseBrace, ETauToken.CloseBrace, ETauToken.Nop);
        }

        protected void AssertSameTokens(string input, params ETauToken[] tokens) {
            var lex = new TauLexer(input);
            lex.Process();
            AssertSameTokens(lex.Tokens, tokens);
        }

        private static void AssertSameTokens(IEnumerable<object> input, params ETauToken[] tokens) {
            var stripped = input.Cast<TauToken>().Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            var expected = stripped.ToList();
            foreach (var token in stripped) {
                Console.Write($"{token}, ");
            }
            Console.WriteLine();
            foreach (var token in tokens) {
                Console.Write($"{token}, ");
            }
            Assert.IsTrue(tokens.SequenceEqual(expected));
        }

        protected static bool IsWhiteSpace(TauToken piToken) {
            switch (piToken.Type) {
                case ETauToken.WhiteSpace:
                case ETauToken.Tab:
                case ETauToken.NewLine:
                    return true;
            }

            return false;
        }
    }
}
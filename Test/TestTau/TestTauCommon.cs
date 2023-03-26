using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pyro.Language.Tau.Lexer;

namespace TestTau {
    public class TestTauCommon {
        protected string LoadTauScript(string fileName) {
            throw new NotImplementedException();
        }

        protected void AssertSameTokens(string input, params ETauToken[] tokens) {
            var toks = new TauLexer(input);
            toks.Process();
            AssertSameTokens(toks.Tokens, tokens);
        }

        private static void AssertSameTokens(IEnumerable<TauToken> input, params ETauToken[] tokens) {
            var stripped = input.Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();
            var expected = stripped.ToList();
            foreach (var token in stripped) {
                Console.Write($"{token}, ");
            }
            Console.WriteLine();
            foreach (var token in tokens) {
                Console.Write($"{token}, ");
            }
            if (tokens.Count() != stripped.Count()) {
                Assert.Fail($"Different size collections: expected {tokens.Length}, got {input.Count()}");
            }
            int i = 0;
            foreach (var next in stripped) {
                if (tokens[i++] != next) {
                    Assert.Fail("Sequences do not match at index " + i);
                }
            }
        }

        private static bool IsWhiteSpace(TauToken piToken) {
            switch (piToken.Type) {
                case ETauToken.WhiteSpace:
                case ETauToken.Tab:
                case ETauToken.NewLine:
                    return true;
                default:
                    return false;
            }
        }
        
    }
}
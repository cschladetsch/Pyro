using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Pyro.Test;
using Pyro.Language.Tau.Lexer;

namespace TestTau {
    public class TestTauCommon
        : TestCommon {
        protected string LoadTauScript(string fileName) {
            return LoadScript(GetFullScriptPathname(fileName));
        }

        protected static void AssertSameTokens(string input, params ETauToken[] tokens) {
            var tauLexer = new TauLexer(input);
            Assert.True(tauLexer.Process());
            AssertSameTokens(tauLexer.Tokens, tokens);
        }

        private static void AssertSameTokens(IEnumerable<TauToken> input, params ETauToken[] tokens) {
            var tauTokens = input as TauToken[] ?? input.ToArray();
            var stripped = tauTokens.Where(t => !IsWhiteSpace(t)).Select(t => t.Type).ToList();

            if (tokens.Count() != stripped.Count()) {
                Assert.Fail($"Different size collections: expected {tokens.Length}, got {stripped.Count}");
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

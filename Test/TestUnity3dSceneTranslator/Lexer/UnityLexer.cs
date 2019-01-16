using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity
{
    public class UnityLexer
        : LexerCommon<EToken, Token, TokenFactory>, ILexer
    {
        public UnityLexer(string input) : base(input)
        {
        }
    }
}
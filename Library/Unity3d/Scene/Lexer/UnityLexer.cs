using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity3d.Scene
{
    public class UnityLexer
        : LexerCommon<EToken, Token, TokenFactory>, ILexer
    {
        public UnityLexer(string input)
            : base(input)
        {
        }
    }
}
using Pyro.Language;
using Pyro.Language.Impl;

namespace Pyro.Unity3d.Scene.Lexer
{
    public class Token
        : TokenBase<EToken>
        , ITokenNode<EToken>
    {
        public Token()
        {
            _type = EToken.None;
        }

        public Token(EToken tok, Slice slice)
            : base(tok, slice)
        {
        }
    }
}

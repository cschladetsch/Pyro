using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity3d.Scene
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
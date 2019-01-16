using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity.Scene
{
    public class Token : TokenBase<EToken>, ITokenNode<EToken>
    {
        public Token(EToken tok, Slice slice)
            : base(tok, slice)
        {
        }

        public Token()
        {
        }
    }
}
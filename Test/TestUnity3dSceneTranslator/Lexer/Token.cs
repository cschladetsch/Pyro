using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity
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
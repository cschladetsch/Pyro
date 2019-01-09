using Diver.Language.Impl;

namespace Diver.Language
{
    public class PiToken 
        : TokenBase<EPiToken>
        , ITokenNode<EPiToken>
    {
        public PiToken()
        {
            _type = EPiToken.None;
        }

        public PiToken(EPiToken type, Slice slice)
            : base(type, slice)
        {
        }
    }
}
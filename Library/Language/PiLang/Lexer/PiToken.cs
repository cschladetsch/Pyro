namespace Diver.Language.PiLang
{
    public class PiToken 
        : TokenBase<EToken>
        , ITokenNode<EToken>
    {
        public PiToken()
        {
            _type = EToken.None;
        }

        public PiToken(EToken type, Slice slice)
            : base(type, slice)
        {
        }
    }
}
namespace Diver.Language.PiLang
{
    public class Token 
        : TokenBase<EToken>
        , ITokenNode<EToken>
    {
        public Token()
        {
            _type = EToken.None;
        }

        public Token(EToken type, Slice slice)
            : base(type, slice)
        {
        }
    }
}
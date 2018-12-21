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

        public Token(EToken type, LexerBase lexer, int ln, Slice slice)
            : base(type, lexer, ln, slice)
        {
        }
    }
}
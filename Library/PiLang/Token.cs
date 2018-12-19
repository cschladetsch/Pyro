namespace Diver.PiLang
{
    public class Token : TokenBase<EToken>
    {
        public Token()
        {
            throw new System.NotImplementedException();
        }

        public Token(EToken type, LexerBase lexer, int ln, Slice slice)
            : base(type, lexer, ln, slice)
        {
        }
    }
}
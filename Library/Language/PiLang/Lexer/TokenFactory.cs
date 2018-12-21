namespace Diver.Language.PiLang
{
    public class TokenFactory :
        ITokenFactory<EToken, Token>
    {
        public Token NewToken(EToken en, int lineNumber, Slice slice)
        {
            return new Token(en, _lexer, lineNumber, slice);
        }

        public Token NewTokenIdent(int lineNumber, Slice slice)
        {
            return NewToken(EToken.Ident, lineNumber, slice);
        }

        public Token NewTokenString(int lineNumber, Slice slice)
        {
            return NewToken(EToken.String, lineNumber, slice);
        }

        public Token NewEmptyToken(int lineNumber, Slice slice)
        {
            return NewToken(EToken.None, lineNumber, slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            _lexer = lexer;
        }

        private LexerBase _lexer;

    }
}

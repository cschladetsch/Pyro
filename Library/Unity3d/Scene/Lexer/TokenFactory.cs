using Pyro.Language;

namespace Pyro.Unity3d.Scene.Lexer
{
    public class TokenFactory
        : ITokenFactory<EToken, Token>
    {
        public Token NewToken(EToken en, Slice slice)
        {
            return new Token(en, slice);
        }

        public Token NewTokenIdent(Slice slice)
        {
            return NewToken(EToken.Ident, slice);
        }

        public Token NewTokenString(Slice slice)
        {
            return NewToken(EToken.String, slice);
        }

        public Token NewEmptyToken(Slice slice)
        {
            return new Token(EToken.None, slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            _lexer = lexer;
        }

        private LexerBase _lexer;
    }
}

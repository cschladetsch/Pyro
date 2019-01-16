using Diver.Language;

namespace Pyro.Unity.Scene
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
            throw new System.NotImplementedException();
        }

        public Token NewTokenString(Slice slice)
        {
            throw new System.NotImplementedException();
        }

        public Token NewEmptyToken(Slice slice)
        {
            return new Token(slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            throw new System.NotImplementedException();
        }
    }
}
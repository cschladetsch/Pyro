using Diver.Language;

namespace Diver.RhoLang
{
    public class RhoTokenFactory
        : ITokenFactory<ERhoToken, RhoToken>
    {
        public RhoToken NewToken(ERhoToken en, Slice slice)
        {
            return new RhoToken(en, slice);
        }

        public RhoToken NewTokenIdent(Slice slice)
        {
            return NewToken(ERhoToken.Ident, slice);
        }

        public RhoToken NewTokenString(Slice slice)
        {
            return NewToken(ERhoToken.String, slice);
        }

        public RhoToken NewEmptyToken(Slice slice)
        {
            return NewToken(ERhoToken.Nop, slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            _lexer = lexer;
        }

        private LexerBase _lexer;
    }
}

namespace Diver.Language.PiLang
{
    /// <summary>
    /// Makes Tokens because C# doesn't allow for constructors that take arguments on generic types
    /// </summary>
    public class PiTokenFactory
        : ITokenFactory<EToken, PiToken>
    {
        public PiToken NewToken(EToken en, Slice slice)
        {
            return new PiToken(en, slice);
        }

        public PiToken NewTokenIdent(Slice slice)
        {
            return NewToken(EToken.Ident, slice);
        }

        public PiToken NewTokenString(Slice slice)
        {
            return NewToken(EToken.String, slice);
        }

        public PiToken NewEmptyToken(Slice slice)
        {
            return NewToken(EToken.None, slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            _lexer = lexer;
        }

        private LexerBase _lexer;
    }
}

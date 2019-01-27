namespace Pyro.Language.Lexer
{
    /// <summary>
    /// Makes Tokens because C# doesn't allow for constructors that take arguments on generic types
    /// </summary>
    public class PiTokenFactory
        : ITokenFactory<EPiToken, PiToken>
    {
        public PiToken NewToken(EPiToken en, Slice slice)
        {
            return new PiToken(en, slice);
        }

        public PiToken NewTokenIdent(Slice slice)
        {
            return NewToken(EPiToken.Ident, slice);
        }

        public PiToken NewTokenString(Slice slice)
        {
            return NewToken(EPiToken.String, slice);
        }

        public PiToken NewEmptyToken(Slice slice)
        {
            return NewToken(EPiToken.None, slice);
        }

        public void SetLexer(LexerBase lexer)
        {
            _lexer = lexer;
        }

        private LexerBase _lexer;
    }
}

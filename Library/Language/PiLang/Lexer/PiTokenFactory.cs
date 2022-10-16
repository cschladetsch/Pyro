namespace Pyro.Language.Lexer {
    /// <inheritdoc />
    /// <summary>
    /// How to make new Pi token nodes given slices and/or token types.
    /// on generic types.
    /// </summary>
    public class PiTokenFactory
        : ITokenFactory<EPiToken, PiToken> {
        private LexerBase _lexer;

        public void SetLexer(LexerBase lexer) => _lexer = lexer;
        public PiToken NewToken(EPiToken en, Slice slice) => new PiToken(en, slice);
        public PiToken NewTokenIdent(Slice slice) => NewToken(EPiToken.Ident, slice);
        public PiToken NewTokenString(Slice slice) => NewToken(EPiToken.String, slice);
        public PiToken NewEmptyToken(Slice slice) => NewToken(EPiToken.None, slice);
    }
}


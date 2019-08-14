namespace Pyro.RhoLang.Lexer
{
    using Language;

    /// <inheritdoc />
    /// <summary>
    /// How to make new Rho token nodes given slices and/or token types.
    /// </summary>
    public class RhoTokenFactory
        : ITokenFactory<ERhoToken, RhoToken>
    {
        private LexerBase _lexer;

        public void SetLexer(LexerBase lexer) => _lexer = lexer;
        public RhoToken NewToken(ERhoToken en, Slice slice) => new RhoToken(en, slice);
        public RhoToken NewTokenIdent(Slice slice) => NewToken(ERhoToken.Ident, slice);
        public RhoToken NewTokenString(Slice slice) => NewToken(ERhoToken.String, slice);
        public RhoToken NewEmptyToken(Slice slice) => NewToken(ERhoToken.Nop, slice);
    }
}


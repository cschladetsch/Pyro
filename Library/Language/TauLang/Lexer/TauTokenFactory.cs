namespace Pyro.TauLang.Lexer { 
    using Language;
    public class TauTokenFactory
        : ITokenFactory<ETauToken, TauToken> {
        private LexerBase _lexer;

        public void SetLexer(LexerBase lexer) => _lexer = lexer;
        public TauToken NewToken(ETauToken en, Slice slice) => new TauToken(en, slice);
        public TauToken NewTokenIdent(Slice slice) => NewToken(ETauToken.Ident, slice);
        public TauToken NewTokenString(Slice slice) => NewToken(ETauToken.String, slice);
        public TauToken NewEmptyToken(Slice slice) => NewToken(ETauToken.Nop, slice);
    }
}

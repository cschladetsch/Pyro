namespace Pyro.Language.Tau.Lexer { 
    public class TauTokenFactory
        : ITokenFactory<ETauToken, TauToken> {

        public TauToken NewToken(ETauToken en, Slice slice) => new TauToken(en, slice);

        public TauToken NewTokenIdent(Slice slice) => NewToken(ETauToken.Identifier, slice);

        public TauToken NewTokenString(Slice slice) => NewToken(ETauToken.String, slice);

        public TauToken NewEmptyToken(Slice slice) => NewToken(ETauToken.Nop, slice);
    }
}

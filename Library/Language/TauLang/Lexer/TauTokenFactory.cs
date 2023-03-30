namespace Pyro.Language.Tau.Lexer {
    public class TauTokenFactory
        : ITokenFactory<ETauToken, TauToken> {
        public TauToken NewToken(ETauToken en, Slice slice) {
            return new TauToken(en, slice);
        }

        public TauToken NewTokenIdent(Slice slice) {
            return NewToken(ETauToken.Identifier, slice);
        }

        public TauToken NewTokenString(Slice slice) {
            return NewToken(ETauToken.String, slice);
        }

        public TauToken NewEmptyToken(Slice slice) {
            return NewToken(ETauToken.Nop, slice);
        }
    }
}
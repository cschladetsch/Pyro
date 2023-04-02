namespace Pyro.Language.Lexer {
    public class PiTokenFactory
        : ITokenFactory<EPiToken, PiToken> {
        public PiToken NewToken(EPiToken en, Slice slice) => new PiToken(en, slice);

        public PiToken NewTokenIdent(Slice slice) => NewToken(EPiToken.Ident, slice);

        public PiToken NewTokenString(Slice slice) => NewToken(EPiToken.String, slice);

        public PiToken NewEmptyToken(Slice slice) => NewToken(EPiToken.None, slice);
    }
}

﻿using Pyro.Language;

namespace Pyro.RhoLang.Lexer {
    /// <inheritdoc />
    /// <summary>
    ///     How to make new Rho token nodes given slices and/or token types.
    /// </summary>
    public class RhoTokenFactory
        : ITokenFactory<ERhoToken, RhoToken> {
        private LexerBase _lexer;

        public RhoToken NewToken(ERhoToken en, Slice slice) {
            return new RhoToken(en, slice);
        }

        public RhoToken NewTokenIdent(Slice slice) {
            return NewToken(ERhoToken.Ident, slice);
        }

        public RhoToken NewTokenString(Slice slice) {
            return NewToken(ERhoToken.String, slice);
        }

        public RhoToken NewEmptyToken(Slice slice) {
            return NewToken(ERhoToken.Nop, slice);
        }

        public void SetLexer(LexerBase lexer) {
            _lexer = lexer;
        }
    }
}
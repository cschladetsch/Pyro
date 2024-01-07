using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyro.Language.PsLang {
    public class PsTokenFactory
        : ITokenFactory<EPsToken, PsToken> {
        public PsToken NewToken(EPsToken en, Slice slice) => new PsToken(en, slice);

        public PsToken NewTokenIdent(Slice slice) => NewToken(EPsToken.Ident, slice);

        public PsToken NewTokenString(Slice slice) => NewToken(EPsToken.String, slice);

        public PsToken NewEmptyToken(Slice slice) => NewToken(EPsToken.None, slice);
    }
}

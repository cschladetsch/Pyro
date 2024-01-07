using Pyro.Language.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyro.Language.PsLang {
    public class PsToken
        : TokenBase<EPsToken>
            , ITokenNode<EPsToken> {

        public PsToken() {
            _type = EPsToken.None;
        }

        public PsToken(EPsToken type, Slice slice)
            : base(type, slice) {
        }
    }
}

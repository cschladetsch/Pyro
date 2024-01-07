using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyro.Language.PsLang {
    public enum EPsToken {
        None,
        Bang,       // !
        Colon,      // :

        Start,      // ^
        End,        // $
        Number,     // int
        Dash,       // -

        Ident,
        String,
    }
}

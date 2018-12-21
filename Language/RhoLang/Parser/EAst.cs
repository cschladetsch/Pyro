using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver.RhoLang
{
    public enum EAst
    {
        None,
        Int,
        String,

        Plus,
        Minus,

        Call,
        Assign,
        Equiv,
        Ident,
        Continuation,
        Lambda,

    }
}

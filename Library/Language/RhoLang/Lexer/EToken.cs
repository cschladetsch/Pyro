using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver.RhoLang.Lexer
{
    public enum EToken
    {
        None,

        Int,
        String,

        Assign,
        Equiv,

        Assert,
        OpenParan,
        Comma,
        CloseParan,
        OpenBrace,
        CloseBrace,

        Tab,
        NewLine,
        Space,
    }
}

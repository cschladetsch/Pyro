using Pyro.Language.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyro.Language.PsLang {
    /// <inheritdoc />
    /// <summary>
    /// Converts a string to a sequence of PsBash tokens.
    /// </summary>
    public class Lexer
        : LexerCommon<EPsToken, PsToken, PsTokenFactory> {

        //private static readonly Dictionary<EOperation, EPiToken> _opToToken = new Dictionary<EOperation, EPiToken>();

        public Lexer(string input)
            : base(input) {
            //CreateOpToToken();
        }
    }
}

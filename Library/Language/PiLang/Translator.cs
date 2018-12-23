using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diver.Exec;

namespace Diver.Language.PiLang
{
    public class Translator : ProcessCommon
    {
        public IRef<Continuation> Continuation;

        public Translator(string input)
        {
            _lexer = new Lexer(input);
            if (_lexer.Failed)

        }

        private Lexer _lexer;
        private Parser _parser;
    }
}

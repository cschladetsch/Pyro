
namespace Pyro.Language.Tau.Parser {
    using Impl;
    using Lexer;
    
    public class TauParser 
        : ParserCommon<TauLexer, TauAstNode, TauToken, ETauToken, ETauAst, TauAstFactory>
        , IParser {
        public TauAstNode Result => _Stack.Peek();

        private readonly EStructure _structure;

        public TauParser(TauLexer lexer, IRegistry reg, EStructure st = EStructure.Namespace)
            : base(lexer, reg) {
            _Current = 0;
            _structure = st;
        }

        public bool Process() {
            if (_Lexer.Failed)
                return Fail(_Lexer.Error);

            RemoveWhitespace();

            return Parse();
        }
        
        private void RemoveWhitespace() {
            var prevNewLine = true;
            foreach (var tok in _Lexer.Tokens) {
                // remove useless consecutive newlines
                var newLine = tok.Type == ETauToken.NewLine;
                if (prevNewLine && newLine)
                    continue;

                prevNewLine = newLine;

                switch (tok.Type) {
                    // keep tabs!
                    case ETauToken.WhiteSpace:
                    case ETauToken.Comment:
                        continue;
                }

                _Tokens.Add(tok);
            }
        }

        private bool Parse() {
            var result = Definition();
            if (Failed || !result)
                return false;

            ConsumeNewLines();

            if (!Try(ETauToken.Nop))
                return FailLocation("Unexpected extra stuff found");

            return _Stack.Count == 1 || InternalFail("Semantic stack not empty after parsing");
        }

        private void ConsumeNewLines() {
            Consume();
        }
        
        private bool Definition() {
            while (!Failed && !Try(ETauToken.Nop))
                if (!Namespace()) {
                    var c = Current();
                    return c.Type == ETauToken.Nop;
                }

            return true;
        }

        private bool Namespace() {
            return true;
        }
    }
}
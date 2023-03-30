using System.Collections.Generic;
using Pyro.Language.Tau.Lexer;

namespace Pyro.Language.Tau.Parser {
    public class TauAstNode {
        private readonly List<TauAstNode> _children = new List<TauAstNode>();

        public readonly TauToken TauToken;
        public ETauAst Type = ETauAst.None;

        public object Value;

        public TauAstNode(ETauAst type) {
            Type = type;
        }

        public TauAstNode(ETauToken type) {
            Type = ETauAst.TokenType;
            TauToken = new TauToken { Type = type };
        }

        public TauAstNode(ETauAst type, TauToken piToken) {
            Type = type;
            TauToken = piToken;
        }

        public TauToken Token => TauToken;

        public IList<TauAstNode> Children => _children;

        public string Text
            => Token?.Text;

        /// <summary>
        ///     Pretty-print.
        /// </summary>
        public override string ToString() {
            var val = $"{Value}";
            var text = $"{Text}";
            var type = $"{Type}: ";

            if (Token?.Type == ETauToken.String) {
                text = $"\"{text}\"";
            }

            if (type == "TokenType: ") {
                type = "";
            }

            return $"{type}{val} {text}";
        }

        public void Add(TauAstNode node) {
            _children.Add(node);
        }

        public void Add(TauToken token) {
            _children.Add(new TauAstNode(ETauAst.TokenType, token));
        }

        public void Add(ETauToken piToken) {
            _children.Add(new TauAstNode(piToken));
        }

        public TauAstNode GetChild(int n) {
            return Children[n];
        }
    }
}
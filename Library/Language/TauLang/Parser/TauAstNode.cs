﻿namespace Pyro.Language.Tau.Parser {
    using System.Collections.Generic;
    using Lexer;

    public class TauAstNode {
        public ETauAst Type = ETauAst.None;
        public TauToken TauToken;
        public TauToken Token => TauToken;
        public object Value;
        public IList<TauAstNode> Children => _children;
        public string Text => Token?.Text;

        private readonly List<TauAstNode> _children = new List<TauAstNode>();

        public TauAstNode(ETauAst type) => Type = type;

        public TauAstNode(ETauToken type) {
            Type = ETauAst.TokenType;
            TauToken = new TauToken() { Type = type };
        }

        public TauAstNode(ETauAst type, TauToken piToken) {
            Type = type;
            TauToken = piToken;
        }

        /// <summary>
        /// Pretty-print.
        /// </summary>
        public override string ToString() {
            var val = $"{Value}";
            var text = $"{Text}";
            var type = $"{Type}: ";

            if (Token?.Type == ETauToken.String)
                text = $"\"{text}\"";

            if (type == "TokenType: ")
                type = "";

            return $"{type}{val} {text}";
        }

        public void Add(TauAstNode node) => _children.Add(node);
        public void Add(TauToken token) => _children.Add(new TauAstNode(ETauAst.TokenType, token));
        public void Add(ETauToken piToken) => _children.Add(new TauAstNode(piToken));
        public TauAstNode GetChild(int n) => Children[n];

    }
}
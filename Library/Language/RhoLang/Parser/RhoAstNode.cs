﻿namespace Pyro.RhoLang.Parser
{
    using System.Collections.Generic;
    using Lexer;

    /// <summary>
    /// A Node in the Rho Abstract Syntax Tree (Ast).
    /// </summary>
    public class RhoAstNode
    {
        public ERhoAst Type = ERhoAst.None;
        public RhoToken RhoToken;
        public RhoToken Token => RhoToken;
        public object Value;
        public IList<RhoAstNode> Children => _children;
        public string Text => Token?.Text;

        private readonly List<RhoAstNode> _children = new List<RhoAstNode>();

        public RhoAstNode(ERhoAst type) => Type = type;

        public RhoAstNode(ERhoToken type)
        {
            Type = ERhoAst.TokenType;
            RhoToken = new RhoToken() { Type = type };
        }

        public RhoAstNode(ERhoAst type, RhoToken piToken)
        {
            Type = type;
            RhoToken = piToken;
        }

        /// <summary>
        /// Pretty-print.
        /// </summary>
        public override string ToString()
        {
            var val = $"{Value}";
            var text = $"{Text}";
            var type = $"{Type}: ";

            if (Token?.Type == ERhoToken.String)
                text = $"\"{text}\"";

            if (type == "TokenType: ")
                type = "";

            return $"{type}{val} {text}";
        }

        public void Add(RhoAstNode node) => _children.Add(node);
        public void Add(RhoToken token) => _children.Add(new RhoAstNode(ERhoAst.TokenType, token));
        public void Add(ERhoToken piToken) => _children.Add(new RhoAstNode(piToken));
        public RhoAstNode GetChild(int n) => Children[n];
    }
}


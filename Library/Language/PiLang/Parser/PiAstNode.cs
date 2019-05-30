using System.Collections.Generic;
using Pyro.Language.Lexer;

namespace Pyro.Language.Parser
{
    /// <summary>
    /// A node in the Abstract Syntax Tree (Ast)
    /// </summary>
    public class PiAstNode
    {
        public EPiAst Type = EPiAst.None;
        public PiToken PiToken;
        public object Value;
        public IList<PiAstNode> Children => _children;

        private readonly List<PiAstNode> _children = new List<PiAstNode>();

        public PiAstNode()
        {
        }

        public PiAstNode(EPiAst type)
        {
            Type = type;
        }

        public PiAstNode(EPiToken type)
        {
            Type = EPiAst.TokenType;
            PiToken = new PiToken() {Type = type};
        }

        public PiAstNode(EPiAst type, PiToken piToken)
        {
            Type = type;
            PiToken = piToken;
        }

        public PiAstNode(EPiAst type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            var text = $"{PiToken}";
            if (string.IsNullOrEmpty(text) && Value != null)
                text = Value.ToString();

            return $"{Type}: '{text}'";
        }

        public void Add(PiAstNode node) => _children.Add(node);

        public void Add(EPiToken piToken) => _children.Add(new PiAstNode(piToken));

        public void Add(EPiAst type, object value) => _children.Add(new PiAstNode(type, value));
    }
}
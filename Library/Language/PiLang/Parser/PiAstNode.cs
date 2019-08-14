namespace Pyro.Language.Parser
{
    using System.Collections.Generic;
    using Lexer;

    /// <summary>
    /// A node in the Abstract Syntax Tree (Ast)
    /// </summary>
    public class PiAstNode
    {
        private readonly List<PiAstNode> _children = new List<PiAstNode>();

        public EPiAst Type;
        public PiToken PiToken;
        public object Value;
        public IList<PiAstNode> Children => _children;

        public PiAstNode(EPiAst type) => Type = type;
        public void Add(PiAstNode node) => _children.Add(node);
        public void Add(EPiToken piToken) => _children.Add(new PiAstNode(piToken));
        public void Add(EPiAst type, object value) => _children.Add(new PiAstNode(type, value));

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
    }
}

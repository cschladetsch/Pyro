using System.Collections.Generic;

namespace Diver.PiLang
{
    internal class AstNode
    {
        public EAstNodeType Type = EAstNodeType.None;
        public Token Token;
        public object Value;
        public List<AstNode> Children => _children;

        public AstNode()
        {
        }

        public AstNode(EAstNodeType type)
        {
            Type = type;
        }

        public AstNode(EToken type)
        {
            Type = EAstNodeType.TokenType;
            Token = new Token() {Type = type};
        }

        public AstNode(EAstNodeType type, Token token)
        {
            Type = type;
            Token = token;
        }

        public AstNode(EAstNodeType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: '{Token}'";
        }

        public void Add(AstNode node)
        {
            _children.Add(node);
        }

        public void Add(EToken token)
        {
            _children.Add(new AstNode(token));
        }
        public void Add(EAstNodeType type, object value)
        {
            _children.Add(new AstNode(type, value));  
        }

        private readonly List<AstNode> _children = new List<AstNode>();
    }
}
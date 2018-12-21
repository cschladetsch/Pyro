using System.Collections.Generic;

namespace Diver.Language.PiLang
{
    public class AstNode
    {
        public EAst Type = EAst.None;
        public Token Token;
        public object Value;
        public IList<AstNode> Children => _children;

        public AstNode()
        {
        }

        public AstNode(EAst type)
        {
            Type = type;
        }

        public AstNode(EToken type)
        {
            Type = EAst.TokenType;
            Token = new Token() {Type = type};
        }

        public AstNode(EAst type, Token token)
        {
            Type = type;
            Token = token;
        }

        public AstNode(EAst type, object value)
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
        public void Add(EAst type, object value)
        {
            _children.Add(new AstNode(type, value));  
        }

        private readonly List<AstNode> _children = new List<AstNode>();
    }
}
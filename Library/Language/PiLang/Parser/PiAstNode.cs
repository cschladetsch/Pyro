using System.Collections.Generic;

namespace Diver.Language
{
    public class PiAstNode
    {
        public EPiAst Type = EPiAst.None;
        public PiToken PiToken;
        public object Value;
        public IList<PiAstNode> Children => _children;

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
            return $"{Type}: '{PiToken}'";
        }

        public void Add(PiAstNode node)
        {
            _children.Add(node);
        }

        public void Add(EPiToken piToken)
        {
            _children.Add(new PiAstNode(piToken));
        }
        public void Add(EPiAst type, object value)
        {
            _children.Add(new PiAstNode(type, value));  
        }

        private readonly List<PiAstNode> _children = new List<PiAstNode>();
    }
}
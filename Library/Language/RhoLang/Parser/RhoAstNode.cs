using System.Collections.Generic;

namespace Diver.Language
{
    public class RhoAstNode
    {
        public ERhoAst Type = ERhoAst.None;
        public RhoToken RhoToken;
        public object Value;
        public IList<RhoAstNode> Children => _children;

        public RhoAstNode()
        {
        }

        public RhoAstNode(ERhoAst type)
        {
            Type = type;
        }

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

        public RhoAstNode(ERhoAst type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: '{RhoToken}'";
        }

        public void Add(RhoAstNode node)
        {
            _children.Add(node);
        }

        public void Add(ERhoToken piToken)
        {
            _children.Add(new RhoAstNode(piToken));
        }
        public void Add(ERhoAst type, object value)
        {
            _children.Add(new RhoAstNode(type, value));
        }

        private readonly List<RhoAstNode> _children = new List<RhoAstNode>();
    }
}

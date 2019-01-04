using System.Collections.Generic;

namespace Diver.Language
{
    /// <summary>
    /// A node in the Rho Abstract Syntax Tree
    /// </summary>
    public class RhoAstNode
    {
        public ERhoAst Type = ERhoAst.None;
        public RhoToken RhoToken;
        public RhoToken Token => RhoToken;
        public object Value;
        public IList<RhoAstNode> Children => _children;
        public string Text => Token?.Text;

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

        //public RhoAstNode(ERhoAst type, object value)
        //{
        //    Type = type;
        //    Value = value;
        //}

        public override string ToString()
        {
            var val = $"{Value}";
            var text = $"{Text}";
            if (!string.IsNullOrEmpty(val))
                val = $"{val}";
            if (!string.IsNullOrEmpty(text))
                text = $"\"{text}\"";
            return $"{Type}: {val} {text}";
        }

        public void Add(RhoAstNode node)
        {
            _children.Add(node);
        }

        public void Add(RhoToken token)
        {
            _children.Add(new RhoAstNode(ERhoAst.TokenType, token));
        }

        public void Add(ERhoToken piToken)
        {
            _children.Add(new RhoAstNode(piToken));
        }

        //public void Add(ERhoAst type, object value)
        //{
        //    _children.Add(new RhoAstNode(type, value));
        //}

        public RhoAstNode GetChild(int n)
        {
            return Children[n];
        }

        private readonly List<RhoAstNode> _children = new List<RhoAstNode>();
    }
}

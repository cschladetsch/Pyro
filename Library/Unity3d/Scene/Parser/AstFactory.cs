using System.Collections.Generic;
using Pyro.Language;
using Pyro.Unity3d.Scene.Lexer;

namespace Pyro.Unity3d.Scene.Parser
{
    public class AstFactory : IAstFactory<Token, AstNode, EAst>
    {
        public void AddChild(AstNode parent, AstNode node)
        {
            parent.Children.Add(node);
        }

        public AstNode New(Token t)
        {
            return new AstNode(t);
        }

        public AstNode New(EAst e, Token t)
        {
            return new AstNode(e, t);
        }

        public AstNode New(EAst t)
        {
            return new AstNode(t);
        }

        public IList<AstNode> GetChildren(AstNode node)
        {
            return node.Children;
        }
    }
}
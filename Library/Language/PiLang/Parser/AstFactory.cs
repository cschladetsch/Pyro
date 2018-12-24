using System.Collections.Generic;

namespace Diver.Language.PiLang
{
    /// <summary>
    /// Ast node factory for Pi lang
    /// </summary>
    public class AstFactory : IAstFactory<PiToken, PiAstNode, EPiAst>
    {
        public void AddChild(PiAstNode parent, PiAstNode node)
        {
            parent.Children.Add(node);
        }

        public void AddChild(PiAstNode parent, object node)
        {
            parent.Children.Add(new PiAstNode(EPiAst.Object, node));
        }

        public PiAstNode New(PiToken piToken)
        {
            return new PiAstNode(EPiAst.TokenType, piToken);
        }

        public PiAstNode New(EPiAst ePi, PiToken t)
        {
            return new PiAstNode(ePi, t);
        }

        public PiAstNode New(EPiAst t)
        {
            return new PiAstNode(t);
        }

        public IList<PiAstNode> GetChildren(PiAstNode node)
        {
            return node.Children;
        }
    }
}
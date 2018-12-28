using System.Collections.Generic;

namespace Diver.Language
{
    /// <summary>
    /// Make RhoAstNodes with various arguments.
    /// Required because C# doesn't allow template types to
    /// take parameters for constructors.
    /// </summary>
    public class RhoAstFactory
        : IAstFactory<RhoToken, RhoAstNode, ERhoAst>
    {
        public void AddChild(RhoAstNode parent, RhoAstNode node)
        {
            parent.Children.Add(node);
        }

        public void AddChild(RhoAstNode parent, object node)
        {
            parent.Children.Add(new RhoAstNode(ERhoAst.TokenType, node));
        }

        public RhoAstNode New(RhoToken t)
        {
            return new RhoAstNode(ERhoAst.TokenType, t);
        }

        public RhoAstNode New(ERhoAst e, RhoToken t)
        {
            return new RhoAstNode(e, t);
        }

        public RhoAstNode New(ERhoAst t)
        {
            return new RhoAstNode(t);
        }

        public IList<RhoAstNode> GetChildren(RhoAstNode node)
        {
            return node.Children;
        }

        //public RhoAstNode New(ERhoToken token)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
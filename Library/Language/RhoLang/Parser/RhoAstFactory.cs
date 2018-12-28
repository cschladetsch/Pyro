using System.Collections.Generic;
using Diver.RhoLang;

namespace Diver.Language
{
    public class RhoAstFactory
        : IAstFactory<RhoToken, RhoAstNode, ERhoAst>
    {
        public void AddChild(RhoAstNode parent, RhoAstNode node)
        {
            parent.Children.Add(node);
        }

        public void AddChild(RhoAstNode parent, object node)
        {
            throw new System.NotImplementedException();
        }

        public RhoAstNode New(RhoToken t)
        {
            throw new System.NotImplementedException();
        }

        public RhoAstNode New(ERhoAst e, RhoToken t)
        {
            throw new System.NotImplementedException();
        }

        public RhoAstNode New(ERhoAst t)
        {
            throw new System.NotImplementedException();
        }

        public IList<RhoAstNode> GetChildren(RhoAstNode node)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using Diver.LanguageCommon;

namespace Diver.PiLang
{
    public class AstFactory : IAstFactory<Token, AstNode, EAst>
    {
        public void AddChild(AstNode parent, AstNode node)
        {
            parent.Children.Add(node);
        }

        public void AddChild(AstNode parent, object node)
        {
            parent.Children.Add(new AstNode(EAst.Object, node));
        }

        public AstNode New(Token token)
        {
            return new AstNode(EAst.TokenType, token);
        }

        //public AstNode New(AstNode e, Token t)
        //{
        //    //AddChild(New());
        //    return null;
        //}

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
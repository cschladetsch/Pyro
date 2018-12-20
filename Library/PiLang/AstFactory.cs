using System;
using Diver.LanguageCommon;

namespace Diver.PiLang
{
    public class AstFactory : IAstFactory<Token, AstNode, EAstNode>
    {
        public void AddChild(AstNode parent, AstNode node)
        {
            parent.Children.Add(node);
        }

        public void AddChild(AstNode parent, object node)
        {
            parent.Children.Add(new AstNode(EAstNode.Object, node));
        }

        public AstNode New(Token token)
        {
            return new AstNode(EAstNode.TokenType, token);
        }

        //public AstNode New(AstNode e, Token t)
        //{
        //    //AddChild(New());
        //    return null;
        //}

        public AstNode New(EAstNode e, Token t)
        {
            return new AstNode(e, t);
        }

        public AstNode New(EAstNode t)
        {
            return new AstNode(t);
        }
    }
}
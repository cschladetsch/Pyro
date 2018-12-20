using System;
using Diver.LanguageCommon;

namespace Diver.PiLang
{
    public class AstFactory : IAstFactory<Token, AstNode, EAstNode>
    {
        public void AddChild(AstNode parent, AstNode node)
        {
            throw new NotImplementedException();
        }

        public void AddChild(AstNode parent, object node)
        {
            throw new NotImplementedException();
        }

        public AstNode New(Token t)
        {
            throw new NotImplementedException();
        }

        public AstNode New(AstNode e, Token t)
        {
            throw new NotImplementedException();
        }

        public AstNode New(EAstNode e, Token t)
        {
            throw new NotImplementedException();
        }

        public AstNode New(EAstNode t)
        {
            throw new NotImplementedException();
        }
    }
}
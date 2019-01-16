using System.Collections.Generic;

namespace Pyro.Unity
{
    public class AstNode
    {
        public EAst Type;
        public Token Token;
        public IList<AstNode> Children = new List<AstNode>();

        public AstNode(Token token)
        {
            Type = EAst.TokenType;
            Token = token;
        }

        public AstNode(EAst type, Token token)
        {
            Type = type;
            Token = token;
        }

        public AstNode(EAst type)
        {
            Type = type;
        }
    }
}
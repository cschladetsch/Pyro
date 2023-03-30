using System.Collections.Generic;
using Pyro.Language.Tau.Lexer;

namespace Pyro.Language.Tau.Parser {
    public class TauAstFactory
        : IAstFactory<TauToken, TauAstNode, ETauAst> {
        public void AddChild(TauAstNode parent, TauAstNode node) {
            parent.Children.Add(node);
        }

        public TauAstNode New(TauToken tokenNode) {
            return new TauAstNode(ETauAst.TokenType, tokenNode);
        }

        public TauAstNode New(ETauAst astEnum, TauToken tokenNode) {
            return new TauAstNode(astEnum, tokenNode);
        }

        public TauAstNode New(ETauAst astEnum) {
            return new TauAstNode(astEnum);
        }

        public IList<TauAstNode> GetChildren(TauAstNode node) {
            return node.Children;
        }

        public void AddChild(TauAstNode parent, TauToken token) {
            parent.Children.Add(New(token));
        }
    }
}
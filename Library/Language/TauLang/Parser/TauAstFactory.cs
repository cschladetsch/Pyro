
namespace Pyro.Language.Tau.Parser {
    using System.Collections.Generic;
    using Lexer;
    public class TauAstFactory
        : IAstFactory<TauToken, TauAstNode, ETauAst> {
        public void AddChild(TauAstNode parent, TauAstNode node) => parent.Children.Add(node);
        public TauAstNode New(TauToken t) => new TauAstNode(ETauAst.TokenType, t);
        public TauAstNode New(ETauAst e, TauToken t) => new TauAstNode(e, t);
        public TauAstNode New(ETauAst t) => new TauAstNode(t);
        public IList<TauAstNode> GetChildren(TauAstNode node) => node.Children;

    }
}
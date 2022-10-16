namespace Pyro.Language.Parser {
    using Lexer;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Ast node factory for Pi-lang.
    /// </summary>
    public class PiAstFactory
        : IAstFactory<PiToken, PiAstNode, EPiAst> {
        public void AddChild(PiAstNode parent, PiAstNode node) => parent.Children.Add(node);
        public IList<PiAstNode> GetChildren(PiAstNode node) => node.Children;
        public PiAstNode New(PiToken piToken) => new PiAstNode(EPiAst.TokenType, piToken);
        public PiAstNode New(EPiAst ePi, PiToken t) => new PiAstNode(ePi, t);
        public PiAstNode New(EPiAst t) => new PiAstNode(t);
    }
}


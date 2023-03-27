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
        public PiAstNode New(PiToken tokenNode) => new PiAstNode(EPiAst.TokenType, tokenNode);
        public PiAstNode New(EPiAst astEnum, PiToken tokenNode) => new PiAstNode(astEnum, tokenNode);
        public PiAstNode New(EPiAst astEnum) => new PiAstNode(astEnum);
    }
}


namespace Pyro.RhoLang.Parser {
    using Language;
    using Lexer;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Make RhoAstNodes with various arguments.
    /// Required because C# doesn't allow template types to
    /// take parameters for constructors.
    /// Also, because Enums in C# can only derive from System.Enum.
    /// </summary>
    public class RhoAstFactory
        : IAstFactory<RhoToken, RhoAstNode, ERhoAst> {
        public void AddChild(RhoAstNode parent, RhoAstNode node) => parent.Children.Add(node);
        public RhoAstNode New(RhoToken tokenNode) => new RhoAstNode(ERhoAst.TokenType, tokenNode);
        public RhoAstNode New(ERhoAst astEnum, RhoToken tokenNode) => new RhoAstNode(astEnum, tokenNode);
        public RhoAstNode New(ERhoAst astEnum) => new RhoAstNode(astEnum);
        public IList<RhoAstNode> GetChildren(RhoAstNode node) => node.Children;
    }
}


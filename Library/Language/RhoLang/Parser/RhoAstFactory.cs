using System.Collections.Generic;

namespace Pyro.RhoLang.Parser
{
    using Language;
    using Lexer;

    /// <inheritdoc />
    /// <summary>
    /// Make RhoAstNodes with various arguments.
    /// Required because C# doesn't allow template types to
    /// take parameters for constructors.
    /// Also, because Enums in C# can only derive from System.Enum.
    /// </summary>
    public class RhoAstFactory
        : IAstFactory<RhoToken, RhoAstNode, ERhoAst>
    {
        public void AddChild(RhoAstNode parent, RhoAstNode node) => parent.Children.Add(node);
        public RhoAstNode New(RhoToken t) => new RhoAstNode(ERhoAst.TokenType, t);
        public RhoAstNode New(ERhoAst e, RhoToken t) => new RhoAstNode(e, t);
        public RhoAstNode New(ERhoAst t) => new RhoAstNode(t);
        public IList<RhoAstNode> GetChildren(RhoAstNode node) => node.Children;
    }
}


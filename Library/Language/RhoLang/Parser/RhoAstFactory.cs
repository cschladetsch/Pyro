using System.Collections.Generic;
using Pyro.Language;
using Pyro.RhoLang.Lexer;

namespace Pyro.RhoLang.Parser {
    /// <inheritdoc />
    /// <summary>
    ///     Make RhoAstNodes with various arguments.
    ///     Required because C# doesn't allow template types to
    ///     take parameters for constructors.
    ///     Also, because Enums in C# can only derive from System.Enum.
    /// </summary>
    public class RhoAstFactory
        : IAstFactory<RhoToken, RhoAstNode, ERhoAst> {
        public void AddChild(RhoAstNode parent, RhoAstNode node) {
            parent.Children.Add(node);
        }

        public RhoAstNode New(RhoToken tokenNode) {
            return new RhoAstNode(ERhoAst.TokenType, tokenNode);
        }

        public RhoAstNode New(ERhoAst astEnum, RhoToken tokenNode) {
            return new RhoAstNode(astEnum, tokenNode);
        }

        public RhoAstNode New(ERhoAst astEnum) {
            return new RhoAstNode(astEnum);
        }

        public IList<RhoAstNode> GetChildren(RhoAstNode node) {
            return node.Children;
        }
    }
}
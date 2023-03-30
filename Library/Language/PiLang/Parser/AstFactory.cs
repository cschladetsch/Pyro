using System.Collections.Generic;
using Pyro.Language.Lexer;

namespace Pyro.Language.Parser {
    /// <inheritdoc />
    /// <summary>
    ///     Ast node factory for Pi-lang.
    /// </summary>
    public class PiAstFactory
        : IAstFactory<PiToken, PiAstNode, EPiAst> {
        public void AddChild(PiAstNode parent, PiAstNode node) {
            parent.Children.Add(node);
        }

        public IList<PiAstNode> GetChildren(PiAstNode node) {
            return node.Children;
        }

        public PiAstNode New(PiToken tokenNode) {
            return new PiAstNode(EPiAst.TokenType, tokenNode);
        }

        public PiAstNode New(EPiAst astEnum, PiToken tokenNode) {
            return new PiAstNode(astEnum, tokenNode);
        }

        public PiAstNode New(EPiAst astEnum) {
            return new PiAstNode(astEnum);
        }
    }
}
using System;
using System.Security.AccessControl;
using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public struct Property {
        public readonly string Type;
        public readonly string Name;
        public readonly string Getter;
        public readonly string Setter;

        public Property(TauAstNode node) {
            Name = node.Text;
            Type = node.Children[0].Text;
            var first = node.Children[1];
            var second = node.Children.Count > 1 ? node.Children[2] : null;

            Getter = MakeTextAccessor(first);
            Setter = MakeTextAccessor(second);
        }

        private static string MakeTextAccessor(TauAstNode accessor) {
            if (accessor == null) {
                return string.Empty;
            }
            if (accessor.Type != ETauAst.TokenType) {
                throw new Exception("Property accessor must be getter or setter");
            }

            switch (accessor.TauToken.Type) {
                case ETauToken.Getter:
                    return "get;";
                case ETauToken.Setter:
                    return "set;";
                default:
                    throw new Exception("Expected getter or setter");
            }
        }
    }
}
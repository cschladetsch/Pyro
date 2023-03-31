using Pyro.Language.Tau.Lexer;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    internal class Property
        : MemberBase {
        public readonly string Type;
        public readonly string Name;
        public readonly string Getter;
        public readonly string Setter;

        public Property(GeneratorBase generatorBase, TauAstNode property)
            : base(generatorBase) {
            Name = property.Text;
            Type = ConvertTypename(property.Children[0].Text);
            var first = property.Children[1];
            var second = property.Children.Count > 2 ? property.Children[2] : null;

            Getter = MakeTextAccessor(first);
            Setter = MakeTextAccessor(second);
        }

        private string MakeTextAccessor(TauAstNode accessor) {
            if (accessor == null) {
                return string.Empty;
            }
            if (accessor.Type != ETauAst.TokenType) {
                _generatorBase.Fail("Property accessor must be getter or setter");
            }

            switch (accessor.TauToken.Type) {
                case ETauToken.Getter:
                    return "get;";
                case ETauToken.Setter:
                    return "set;";
                default:
                    _generatorBase.Fail("Expected getter or setter");
                    return string.Empty;
            }
        }
    }
}
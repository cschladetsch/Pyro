using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public struct Property {
        public readonly string Type;
        public readonly string Name;
        public readonly string Getter;
        public readonly string Setter;

        public Property(TauAstNode node) {
            Name = node.Text;
            Type = node.Children[1].Text;
            var first = node.Children[0];
            var second = node.Children.Count > 1 ? node.Children[2] : null;

            Getter = MakeTextAccessor(first);
            Setter = MakeTextAccessor(second);
        }

        private static string MakeTextAccessor(TauAstNode accessor) {
            if (accessor == null) {
                return string.Empty;
            }

            var text = accessor.Text;
            switch (text) {
                case "get":
                    return "get;";
                case "set":
                    return "set;";
                default:
                    return string.Empty;
            }
        }
    }
}
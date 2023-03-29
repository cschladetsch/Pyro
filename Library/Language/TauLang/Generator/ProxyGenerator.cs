using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class ProxyGenerator 
        : GeneratorBase {

        public ProxyGenerator(TauParser parser)
            : base(parser) {
        }

        public bool Process() {
            if (Parser.Failed) {
                return Fail(Parser.Error);
            }

            return GenerateProxies(Parser.Result) && !Failed;
        }

        private bool GenerateProxies(TauAstNode node) {
            switch (node.Type) {
                case ETauAst.Namespace:
                    return GenerateNamespace(node);
            }

            return false;
        }

        private bool GenerateNamespace(TauAstNode node) {
            _stringBuilder.AppendLine($"namespace {node.Text} {{");
            foreach (var @interface in node.Children) {
                if (@interface.Type != ETauAst.Interface) {
                    return FailLocation(@interface, "Expected interface");
                }

                if (!GenerateInterface(@interface)) {
                    return false;
                }
            }

            _stringBuilder.AppendLine("}");
            return !Failed;
        }

        private bool GenerateInterface(TauAstNode @interface) {
            _stringBuilder.AppendLine($"    interface {@interface.Text} {{");
            foreach (var member in @interface.Children) {
                switch (member.Type) {
                    case ETauAst.Method:
                        if (!GenerateMethod(member)) {
                            return false;
                        }
                        break;
                    case ETauAst.Property:
                        if (!GenerateProperty(member)) {
                            return false;
                        }
                        break;
                }
            }

            _stringBuilder.AppendLine("    }");
            return true;
        }

        private bool GenerateProperty(TauAstNode property) {
            if (property.Children.Count < 2) {
                return FailLocation(property, "Property needs at least a name and a getter and/or setter");
            }

            var name = property.Text;
            var type = property.Children[0];
            var first = property.Children[1];
            var second = property.Children.Count > 2 ? property.Children[2] : null;

            var getText = MakeTextAccessor(first);
            var setText = MakeTextAccessor(second);

            _stringBuilder.AppendLine($"        public IFuture<{type}> {name} {{ {getText} {setText} }}");
            
            return true;
        }

        private string MakeTextAccessor(TauAstNode node) {
            if (node == null) {
                return string.Empty;
            }

            switch (node.Type) {
                case ETauAst.Getter:
                    return "get; ";
                case ETauAst.Setter:
                    return "set; ";
                default:
                    return string.Empty;
            }
        }

        private bool GenerateMethod(TauAstNode member) {
            throw new System.NotImplementedException();
        }

        private bool FailLocation(TauAstNode node, string text) {
            Error = $"Generator Error: {text}: {node}";
            return false;
        }
    }
}

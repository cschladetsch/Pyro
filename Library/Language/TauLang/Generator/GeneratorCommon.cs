using System;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class GeneratorCommon
        : GeneratorBase
        , IGeneratorCommon {
        private const string _Prelude = @"
using System;
using System.Collections.Generic;

using Flow;

using Pyro;
using Pyro.Network;
";

        private readonly EGeneratedType _classType;
        private readonly TauParser _parser;
        protected virtual string _baseType { get; }

        protected GeneratorCommon(TauParser parser, EGeneratedType classType)
            : base(parser) {
            AppendLine(_Prelude);
            _classType = classType;
            _parser = parser;
        }

        public override bool Run() {
            return GenerateNamespace(_parser.Result);
        }

        public bool GenerateNamespace(TauAstNode node) {
            Append($"namespace Pyro.Network.{node.Text}{GetQualifier()} {{\n");
            foreach (var @interface in node.Children) {
                if (@interface.Type != ETauAst.Interface) {
                    return FailLocation(@interface, "Expected interface");
                }

                if (!GenerateInterface(@interface)) {
                    return false;
                }
            }

            AppendLine("}");
            return !Failed;
        }

        private string GetQualifier() {
            return _classType.GetGeneratedTypeString();
        }

        public virtual bool GenerateInterface(TauAstNode @interface) {
            _stringBuilder.AppendLine($"    public interface I{@interface.Text}{GetQualifier()} : {_baseType} {{\n");
            foreach (var member in @interface.Children)
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

                    case ETauAst.Event:
                        throw new NotImplementedException("Events");

                    default:
                        return Fail($"Unexpected token {member} in proxy interface.");
                }

            AppendLine("    }");
            return !Failed;
        }

        public virtual bool GenerateProperty(TauAstNode property) {
            if (property.Children.Count < 1) {
                return FailLocation(property, "Property needs at least a name and a getter and/or setter");
            }

            var info = new Property(property);
            AppendLine($"        {GenerateReturn(info.Type)} {info.Name} {{ {info.Getter} {info.Setter} }}");

            return true;
        }

        public virtual bool GenerateMethod(TauAstNode member) {
            var info = new Method(this, member);
            AppendLine($"        {GenerateReturn(info.Type)} {info.Name}({info.ParameterText});");
            return true;
        }

        public bool FailLocation(TauAstNode node, string text) {
            return _parser.FailLocation(text);
        }

        private string GenerateReturn(string infoType) {
            return _classType == EGeneratedType.EAgent ? infoType : $"IFuture<{infoType}>";
        }
    }
}
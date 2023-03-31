using System.Linq;
using System.Text;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    internal class Method {
        private readonly GeneratorBase _generatorBase;
        internal readonly string Name;
        internal readonly string ParameterText;
        internal readonly string Type;

        internal Method(GeneratorBase generatorBase, TauAstNode member) {
            _generatorBase = generatorBase;
            Name = member.Text;
            Type = member.Children[0].Text;
            ParameterText = BuildParameterText(member.Children[1]);
        }

        private string BuildParameterText(TauAstNode parameters) {
            var stringBuilder = new StringBuilder();
            if (parameters.Children.Count % 2 != 0) {
                _generatorBase.Fail("Parameters must be a set of (type name)*");
                return string.Empty;
            }

            var args = parameters.Children;
            var comma = "";
            for (var n = 0; n < args.Count(); n += 2) {
                var type = args[n].Text;
                var name = args[n + 1].Text;
                stringBuilder.Append($"{comma}{type} {name}");
                comma = ", ";
            }

            return stringBuilder.ToString();
        }
    }
}
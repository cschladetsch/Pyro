using System.Linq;
using System.Text;

using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    internal class Method
        : MemberBase {
        internal readonly string Name;
        internal readonly string ParameterListString;
        internal readonly string Type;

        internal Method(GeneratorBase generatorBase, TauAstNode method)
            : base(generatorBase) {
            Name = method.Text;
            Type = ConvertTypename(method.Children[0].Text);
            ParameterListString = BuildParameterListString(method.Children[1]);
        }

        private string BuildParameterListString(TauAstNode parameters) {
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
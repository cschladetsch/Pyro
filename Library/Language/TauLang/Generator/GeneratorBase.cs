using System.Text;

using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class GeneratorBase
        : Process {
        public TauParser Parser { get; }

        protected readonly StringBuilder _stringBuilder = new StringBuilder();

        public string Result => _stringBuilder.ToString();

        protected GeneratorBase(TauParser parser) {
            Parser = parser;
        }

        public object RemoveQuotes(TauAstNode node) {
            var text = node.Text;
            if (text.StartsWith("\"") && text.EndsWith("\"")) {
                return text.Substring(1, text.Length - 1);
            }

            return text;
        }

        protected void Append(string text) {
            _stringBuilder.Append(text);
        }

        protected void AppendLine(string text) {
            _stringBuilder.AppendLine(text);
        }
    }
}
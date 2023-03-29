using System.Text;

using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class GeneratorBase
        : Process {
        protected TauParser Parser { get; }

        protected readonly StringBuilder _stringBuilder = new StringBuilder();

        public string Result => _stringBuilder.ToString();

        protected GeneratorBase(TauParser parser) {
            Parser = parser;
        }
    }
}
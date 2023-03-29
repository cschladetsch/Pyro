using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class AgentGenerator
        : GeneratorBase {
        public AgentGenerator(TauParser parser)
            : base(parser) {
        }

        public bool Process() {
            if (Parser.Failed) {
                return Fail($"Agent Generator Failed because Parser failed with '{Parser.Error}");
            }

            return GenerateAgents(Parser.Result) && !Failed;
        }

        private bool GenerateAgents(TauAstNode node) {
            return false;
        }
    }
}

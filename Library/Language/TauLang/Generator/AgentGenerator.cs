using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class AgentGenerator
        : GeneratorCommon {
        public AgentGenerator(TauParser parser)
            : base(parser, EGeneratedType.EAgent) {
        }
    }
}
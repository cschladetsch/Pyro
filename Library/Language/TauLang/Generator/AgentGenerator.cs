using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class AgentGenerator
        : GeneratorCommon {
        protected override string _baseType => "IAgentBase";

        public AgentGenerator(TauParser parser)
            : base(parser, EGeneratedType.EAgent) {
        }
    }
}
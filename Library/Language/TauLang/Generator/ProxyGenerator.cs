using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class ProxyGenerator 
        : GeneratorCommon {
        public ProxyGenerator(TauParser parser)
            : base(parser, EGeneratedType.EProxy) {
        }
    }
}

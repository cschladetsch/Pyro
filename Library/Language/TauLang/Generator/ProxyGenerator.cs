using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class ProxyGenerator
        : GeneratorCommon {
        protected override string _baseType => "IProxyBase";

        public ProxyGenerator(TauParser parser)
            : base(parser, EGeneratedType.EProxy) {
        }
    }
}
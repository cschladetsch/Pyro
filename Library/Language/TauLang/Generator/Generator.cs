using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau.Generator {
    internal class Generator
        : Process {
        private AgentGenerator _agentGenerator = new AgentGenerator();
        private ProxyGenerator _proxyGenerator = new ProxyGenerator();
        private readonly TauParser _parser;

        public Generator(TauParser parser) {
            _parser = parser;
        }

        public bool Process() {
            if (_parser.Failed) {
                return Fail(_parser.Error);
            }

            GenerateProxies();
            GenerateAgents();
            return !Failed;
        }

        private void GenerateAgents() {
            throw new System.NotImplementedException();
        }

        private void GenerateProxies() {
            throw new System.NotImplementedException();
        }
    }
}

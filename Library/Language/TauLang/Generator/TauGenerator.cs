using System.Threading;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class TauGenerator
        : Process {
        private readonly TauParser _parser;

        public TauGenerator(TauParser parser) {
            _parser = parser;
        }

        public bool Process(bool generateAgents, bool generateProxies) {
            if (_parser.Failed) {
                return Fail($"Generator failed because Parser error: {_parser.Error}");
            }

            if (generateProxies && !GenerateProxies()) {
                return false;
            }

            if (generateAgents && !GenerateAgents()) {
                return false;
            }

            return !Failed;
        }

        private bool GenerateAgents() {
            var agentGenerator = new AgentGenerator(_parser);
            return !agentGenerator.Process() ? Fail($"Generator Failed: {agentGenerator.Error}") : WriteFile("Agents", agentGenerator.Result);
        }

        private bool WriteFile(string baseFolder, string contents) {
            return false;
        }

        private bool GenerateProxies() {
            var proxyGenerator = new ProxyGenerator(_parser);
            return !proxyGenerator.Process() ? Fail($"Generator Failed: {proxyGenerator.Error}") : WriteFile("Agents", proxyGenerator.Result);
        }
    }
}

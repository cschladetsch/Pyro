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
            return Generate("Agents", new AgentGenerator(_parser));
        }

        private bool GenerateProxies() {
            return Generate("Proxies", new ProxyGenerator(_parser));
        }

        private bool Generate(string baseFolder, GeneratorBase generator) {
            return !generator.Run()
                ? Fail($"Generator Failed: {generator.Error}")
                : WriteFile(baseFolder, generator.Result);
        }

        private bool WriteFile(string baseFolder, string contents) {
            return false;
        }
    }
}

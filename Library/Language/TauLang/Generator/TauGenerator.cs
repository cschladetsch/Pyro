using System.IO;
using Pyro.Language.Tau.Parser;

namespace Pyro.Language.Tau {
    public class TauGenerator
        : Process {
        private readonly TauParser _parser;

        public TauGenerator(TauParser parser) {
            _parser = parser;
        }

        public bool Process(string baseName, bool generateAgents, bool generateProxies) {
            if (_parser.Failed) {
                return Fail($"Generator failed because Parser error: {_parser.Error}");
            }

            if (generateProxies && !GenerateProxies(baseName + "Proxy.cs")) {
                return false;
            }

            if (generateAgents && !GenerateAgents(baseName + "Agent.cs")) {
                return false;
            }

            return !Failed;
        }

        private bool GenerateAgents(string fileName) {
            return Generate(fileName, new AgentGenerator(_parser));
        }

        private bool GenerateProxies(string fileName) {
            return Generate(fileName, new ProxyGenerator(_parser));
        }

        private bool Generate(string fileName, GeneratorCommon generator) {
            return !generator.Run()
                ? Fail($"Generator Failed: {generator.Error}")
                : WriteFile(fileName, generator.Result);
        }

        private bool WriteFile(string fileName, string contents) {
            File.WriteAllText(fileName, contents);
            return true;
        }
    }
}

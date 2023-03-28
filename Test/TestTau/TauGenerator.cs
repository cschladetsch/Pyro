using System;
using Pyro;
using Pyro.Language.Tau.Parser;

namespace TestTau {
    public class TauGenerator
        : Process {
        private readonly TauParser _parser;

        public TauGenerator(TauParser parser) {
            _parser = parser;
        }

        public bool Process() {
            if (_parser.Failed) {
                return Fail(_parser.Error);
            }

            return !Failed;
        }
    }
}
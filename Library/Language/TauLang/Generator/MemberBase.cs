namespace Pyro.Language.Tau {
    public class MemberBase {
        protected readonly GeneratorBase _generatorBase;

        public MemberBase(GeneratorBase generatorBase) {
            _generatorBase = generatorBase;
        }

        protected string ConvertTypename(string name) {
            return name == "void" ? "Void" : name;
        }
    }
}
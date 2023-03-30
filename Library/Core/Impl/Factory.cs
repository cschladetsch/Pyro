namespace Pyro.Impl {
    internal class Factory
        : IFactory {
        public IRegistry NewRegistry() {
            return new Registry();
        }

        public ITree NewTree() {
            return new Tree();
        }
    }
}
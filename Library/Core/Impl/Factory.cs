namespace Pyro.Impl {
    internal class Factory
        : IFactory {
        public IRegistry NewRegistry() => new Registry();
        public ITree NewTree() => new Tree();
    }
}

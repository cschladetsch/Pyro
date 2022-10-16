namespace Pyro {
    using Flow.Impl;

    public class ReflectedBase
        : Generator
        , IReflected {
        public IRefBase SelfBase { get; set; }
    }
}


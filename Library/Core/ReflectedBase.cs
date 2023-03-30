using Flow.Impl;

namespace Pyro {
    public class ReflectedBase
        : Generator
            , IReflected {
        public IRefBase SelfBase { get; set; }
    }
}
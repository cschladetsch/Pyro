namespace Pyro.Exec {
    /// <summary>
    /// Register fundamental types in a registry.
    /// </summary>
    public static class RegisterTypes {
        public static void Register(IRegistry reg) {
            reg.Register(new ClassBuilder<Continuation>(reg, Continuation.ToText)
                .Class);
        }
    }
}


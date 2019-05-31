namespace Pyro.Exec
{
    public static class RegisterTypes
    {
        public static void Register(IRegistry reg)
        {
            reg.Register(new ClassBuilder<Continuation>(reg, Continuation.ToText)
                .Class);
        }
    }
}


using Diver.Impl;

namespace Diver.BuiltinTypes
{
    internal static class Builtins
    {
        internal static void Register(IRegistry reg)
        {
            RegisterString(reg);
        }

        internal static void RegisterString(IRegistry reg)
        {
            reg.Register(new ClassBuilder<string>(reg)
                .Methods
                    .Add<int,int,string>("Substring", (s,n,m) => s.Substring(n,m))
                    .Add<int,string>("Substring1", (s,n) => s.Substring(n))
                .Class);
        }
    }
}

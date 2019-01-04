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
                    .Add<int,int,string>("Substring", (s,n,m) => reg.Get<string>(s).Substring(reg.Get<int>(n), reg.Get<int>(m)))
                    .Add<int,string>("Substring1", (s,n) => reg.Get<string>(s).Substring(reg.Get<int>(n)))
                .Class);
        }

    }
}

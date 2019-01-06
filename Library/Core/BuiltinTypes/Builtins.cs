using System.Collections.Generic;
using Diver.Impl;

namespace Diver.BuiltinTypes
{
    internal static class Builtins
    {
        public class Void
        {
        }

        internal static void Register(IRegistry reg)
        {
            reg.Register(new Class<Void>(reg));
            reg.Register(new Class<bool>(reg));
            reg.Register(new Class<int>(reg));
            RegisterString(reg);
            reg.Register(new Class<float>(reg));
            reg.Register(new Class<List<object>>(reg));
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

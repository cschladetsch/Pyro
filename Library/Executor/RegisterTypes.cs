using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver.Exec
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

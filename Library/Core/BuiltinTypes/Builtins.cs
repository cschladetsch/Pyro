using System.Collections.Generic;
using System.Text;
using Diver.Impl;

namespace Diver
{
    public class Void
    {
    }

    internal static class BuiltinTypes
    {
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
            reg.Register(new ClassBuilder<List<object>>(reg, ListToText)
                .Class);
            reg.Register(new ClassBuilder<string>(reg, StringToText)
                .Methods
                    .Add<int,int,string>("Substring", (s,n,m) => s.Substring(n,m))
                    .Add<int,string>("Substring1", (s,n) => s.Substring(n))
                .Class);
        }

        private static void ListToText(IRegistry reg, StringBuilder arg1, List<object> arg2)
        {
            arg1.Append('[');
            foreach (var obj in arg2)
            {
                reg.AppendText(arg1, obj);
            }
            arg1.Append(']');
        }

        private static void StringToText(IRegistry reg, StringBuilder arg1, string arg2)
        {
            arg1.Append('"');
            arg2 = arg2.Replace("\"", "\\\"");
            arg2 = arg2.Replace("~", "\\~");
            arg1.Append(arg2);
            arg1.Append('"');
        }
    }
}

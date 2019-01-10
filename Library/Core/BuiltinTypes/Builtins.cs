using System.Collections;
using System.Collections.Generic;
using System.Text;
using Diver.Impl;
using NUnit.Framework;

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
            reg.Register(new ClassBuilder<string>(reg, StringToText)
                .Methods
                    .Add<int,int,string>("Substring", (s,n,m) => s.Substring(n,m))
                    .Add<int,string>("Substring1", (s,n) => s.Substring(n))
                .Class);
            reg.Register(new Class<float>(reg));
            reg.Register(new ClassBuilder<List<object>>(reg, ListToText)
                .Class);
        }

        private static void ListToText(IRegistry reg, StringBuilder arg1, IList arg2)
        {
            arg1.Append('[');
            var first = true;
            foreach (var obj in arg2)
            {
                if (first)
                    first = false;
                else
                    arg1.Append(' ');
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

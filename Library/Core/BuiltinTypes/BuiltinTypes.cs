using System.Collections;
using System.Collections.Generic;
using System.Text;
using Pyro.Impl;

namespace Pyro.BuiltinTypes
{
    public static class BuiltinTypes
    {
        public static void Register(IRegistry reg)
        {
            reg.Register(new Class<Void>(reg));
            reg.Register(new Class<bool>(reg));
            reg.Register(new Class<int>(reg));
            reg.Register(new ClassBuilder<string>(reg, StringToText)
                .Methods
                    .Add<int, int, string>("Substring", (s, m, n) => s.Substring(n, m))
                    .Add<int, string>("Substring1", (s, n) => s.Substring(n))
                .Class);
            reg.Register(new Class<float>(reg));
            reg.Register(new ClassBuilder<List<object>>(reg, ListToText)
                .Class);
            reg.Register(new ClassBuilder<Dictionary<object, object>>(reg, DictToText)
                .Class);
        }

        private static void DictToText(IRegistry reg, StringBuilder sb, Dictionary<object, object> dict)
        {
            foreach (var kv in dict)
            {
                reg.ToPiScript(sb, kv.Key);
                sb.Append(' ');
                reg.ToPiScript(sb, kv.Value);
                sb.Append(' ');
            }

            sb.Append($"{dict.Count} tomap ");
        }

        private static void ListToText(IRegistry reg, StringBuilder sb, IList list)
        {
            sb.Append($"{list.Count}: [");
            var first = true;
            foreach (var obj in list)
            {
                if (first)
                    first = false;
                else
                    sb.Append(' ');
                reg.ToPiScript(sb, obj);
            }

            sb.Append(']');
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

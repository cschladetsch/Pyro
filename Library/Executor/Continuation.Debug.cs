using System;
using System.Text;
using Diver.Exec;

namespace Diver.Exec
{
    public partial class Continuation : Reflected<Continuation>
    {
        public void DebugWrite(StringBuilder str)
        {
            if (_scope == null)
            {
                str.AppendLine("    No scope");
            }
            else
            {
                str.AppendLine("    Scope:");
                foreach (var obj in _scope)
                {
                    str.AppendLine($"    {obj.Key}={obj.Value}");
                }
            }

            if (_code == null)
            {
                str.AppendLine("    No code");
                return;
            }

            if (_next == _code.Count)
            {
                str.AppendLine("    [end]");
                return;
            }

            var index = Math.Max(0, _next - 6);
            str.AppendLine($"    Code from {index} to fault at {_next}/{_code.Count}:");
            str.Append("        ");
            for (int n = index; n <= _next; ++n)
            {
                str.Append($"{_code[n]}, ");
            }
        }
    }
}

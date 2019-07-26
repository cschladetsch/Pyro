namespace Pyro.Exec
{
    using System;
    using System.Text;

    public partial class Continuation
        : Reflected<Continuation>
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

            if (Code == null)
            {
                str.AppendLine("    No code");
                return;
            }

            if (Ip == Code.Count)
            {
                str.AppendLine("    [end]");
                return;
            }

            var index = Math.Max(0, Ip - 6);
            str.AppendLine($"    Code from {index} to fault at {Ip}/{Code.Count}:");
            str.Append("        ");
            for (var n = index; n <= Ip; ++n)
            {
                str.Append($"{Code[n]}, ");
            }
        }
    }
}


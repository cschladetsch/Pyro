namespace Pyro.Exec {
    using System.Text;

    public partial class Continuation
        : Reflected<Continuation> {
        public void DebugWrite(StringBuilder str) {
            if (_scope == null || _scope.Count == 0) {
                str.AppendLine("\tNothing in scope.");
            } else {
                str.AppendLine("\tScope:");
                foreach (var obj in _scope)
                    str.AppendLine($"\t\tname={obj.Key}, val={obj.Value}");
            }

            if (Code == null) {
                str.AppendLine("\tNo code");
                return;
            }

            str.Append($"\tIp={Ip}/{Code.Count}, Active={Active}, Run={Running}: ");
            for (var n = 0; n < Code.Count; ++n) {
                string wrap1 = "", wrap2 = "";
                if (n == Ip) {
                    wrap1 = " >>> ";
                    wrap2 = " <<< ";
                }
                str.Append($"{wrap1}{Code[n]}{wrap2}, ");
            }
        }

    }
}


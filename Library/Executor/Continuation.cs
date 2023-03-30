using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyro.Exec {
    /// <summary>
    ///     Also known as a co-routine.
    ///     Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation {
        public delegate void ContinuationHandler(Continuation continuation);

        public delegate void ContinuationIpChangedHandler(Continuation continuation, int last, int current);

        private int _ip;

        public Continuation() {
            Active = true;
            Running = false;
        }

        public Continuation(IList<object> code)
            : this() {
            Code = code;
        }

        /// <summary>
        ///     The 'instruction pointer', or the thing to execute next in list of objects in code block.
        /// </summary>
        public int Ip {
            get => _ip;
            private set {
                OnIpChanged?.Invoke(this, _ip, value);
                _ip = value;
            }
        }

        public IList<object> Code { get; }

        private IList<string> Args { get; set; }

        private IDictionary<string, object> _scope => Scope;

        public event ContinuationHandler OnScopeChanged;

        public event ContinuationHandler OnLeave;

        public event ContinuationIpChangedHandler OnIpChanged;

        internal void FireOnLeave() {
            OnLeave?.Invoke(this);
        }

        /// <summary>
        ///     Helper to make a new continuation, which also uses a referenced list for scope
        /// </summary>
        public new static Continuation New(IRegistry reg) {
            var code = reg.Add(new List<object>());
            return reg.Add(new Continuation(code.Value)).Value;
        }

        public static void ToText(IRegistry reg, StringBuilder str, Continuation cont) {
            str.Append('{');
            foreach (var elem in cont.Code) {
                switch (elem) {
                    case EOperation op:
                        str.Append(OpToText(op));
                        break;

                    case bool val:
                        str.Append(val ? "true" : "false");
                        break;

                    default:
                        reg.ToPiScript(str, elem);
                        break;
                }

                str.Append(' ');
            }

            str.Append('}');
        }

        // this is human-readable version. for transmission/persistence, use ToPiScript()
        public override string ToString() {
            var str = new StringBuilder();
            str.Append('{');
            str.Append($"#{Ip}/{Code.Count} ");

            if (Args != null) {
                str.Append('(');
                var comma = "";
                foreach (var a in Args) {
                    str.Append($"{a}{comma}");
                    comma = ", ";
                }

                str.Append(") ");
            }

            foreach (var c in Code) {
                str.Append(c);
                str.Append(", ");
            }

            str.Append('}');

            return str.ToString();
        }

        public void AddArg(string ident) {
            if (Args == null) {
                Args = new List<string>();
            }

            Args.Add(ident);
        }

        public void Enter(Executor exec) {
            if (Ip == Code.Count) {
                Ip = 0;
            }

            if (Args != null) {
                if (exec.DataStack.Count < Args.Count) {
                    throw new DataStackEmptyException($"Expected at least {Args.Count} objects on stack.");
                }

                foreach (var arg in Args.Reverse())
                    Scope[arg] = exec.DataStack.Pop();
            }

            OnScopeChanged?.Invoke(this);
        }

        public bool HasScopeObject(string label) {
            return _scope.ContainsKey(label);
        }

        public void SetScopeObject(string label, object val) {
            _scope[label] = val;
            OnScopeChanged?.Invoke(this);
        }

        public object FromScope(string label) {
            return _scope.TryGetValue(label, out var value) ? value : null;
        }

        public bool Next(out object next) {
            var has = Ip < Code.Count;
            next = has ? Code[Ip++] : null;
            if (has) {
                return true;
            }

            Ip = 0;
            return false;
        }
    }
}
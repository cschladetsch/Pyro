using System.Collections.Generic;
using System.Text;

namespace Pyro.Exec
{
    /// <summary>
    /// Also known as a co-routine. Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation
        : Reflected<Continuation>
    {
        public IList<object> Code => _code;
        public IList<string> Args => _args;
        /// <summary>
        /// The 'instruction pointer', or the thing to execute next in list of objects in code block
        /// </summary>
        public int Ip => _next;

        public Continuation(IList<object> code)
        {
            _code = code;
        }

        /// <summary>
        /// Helper to make a new continuation, which also uses a referenced list for scope
        /// </summary>
        public static Continuation New(IRegistry reg)
        {
            var code = reg.Add(new List<object>());
            return reg.Add(new Continuation(code.Value)).Value;
        }

        public static void ToText(IRegistry reg, StringBuilder str, Continuation cont)
        {
            str.Append('{');
            foreach (var elem in cont.Code)
            {
                switch (elem)
                {
                    case EOperation op:
                        str.Append('`');
                        str.Append((int) op);
                        break;
                    case bool val:
                        str.Append(val ? "true" : "false");
                        break;
                    default:
                        reg.AppendText(str, elem);
                        break;
                }

                str.Append(' ');
            }
            str.Append('}');
        }

        public static void Register(IRegistry reg)
        {
            reg.Register(new ClassBuilder<Continuation>(reg, ToText)
                .Class);
        }

        // this is human-readable version. for transmission/persistence, use ToText()
        public override string ToString()
        {
            var str = new StringBuilder();
            //str.Append($"Continuation: {_scope.Count} args, {_code.Count} instructions:\n\t");
            str.Append('{');
            str.Append($"#{_next}/{_code.Count} ");
            if (_args != null)
            {
                str.Append('(');
                var comma = "";
                foreach (var a in _args)
                {
                    str.Append($"{a}{comma}");
                    comma = ", ";
                }
                str.Append(") ");
            }

            foreach (var c in _code)
            {
                str.Append(c);
                str.Append(", ");
            }
            str.Append('}');

            return str.ToString();
            //return $"#{_next}/{_code.Count}:{ToText()}";
        }

        public void AddArg(string ident)
        {
            if (_args == null)
                _args = new List<string>();
            _args.Add(ident);
        }

        public void Enter(Executor exec)
        {
            if (_args == null)
                return;

            // already entered
            if (_next != 0)
                return;

            if (exec.DataStack.Count < _args.Count)
                throw new DataStackEmptyException();
            
            foreach (var arg in _args)
                _scope[arg] = exec.DataStack.Pop();

            _next = 0;
        }

        public bool HasScopeObject(string label)
        {
            return _scope.ContainsKey(label);
        }

        public void SetScopeObject(string label, object val)
        {
            _scope[label] = val;
        }

        public object FromScope(string label)
        {
            return _scope.TryGetValue(label, out var value) ? value : null;
        }

        public bool Next(out object next)
        {
            var has = _next < _code.Count;
            next = has ? _code[_next++] : null;
            if (!has)
                Reset();
            return has;
        }

        public void Reset()
        {
            // TODO: want to reset scope here, but also want to keep it to check results in unit-tests
            //_scope.Clear();
            _next = 0;
        }

        private int _next;
        private IList<string> _args;
        private readonly IList<object> _code;
        private IDictionary<string, object> _scope => Scope;
    }
}

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace Diver.Exec
{
    /// <summary>
    /// Also known as a co-routine. Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation : Reflected<Continuation>
    {
        public IList<object> Code => _code;//{ get => _code; set => _code = value; }
        public IDictionary<string, object> Scope => _scope;//{ get => _scope; set => _scope = value; }
        public IList<string> Args => _args;

        public Continuation(List<object> code)
        {
            _code = code;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append($"Continuation: {_scope.Count} args, {_code.Count} instructions:\n\t");
            foreach (var c in _code)
            {
                str.Append(c);
                str.Append(", ");
            }

            return str.ToString();
        }

        public void AddArg(string ident)
        {
            _args.Add(ident);
        }

        public void Enter(Executor exec)
        {
            foreach (var arg in _args)
            {
                _scope[arg] = exec.DataStack.Pop();
            }

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
            return has;
        }

        public void Reset()
        {
            _next = 0;
        }

        private int _next;
        private List<string> _args;
        private readonly List<object> _code;
        private readonly Dictionary<string, object> _scope = new Dictionary<string, object>();
    }
}
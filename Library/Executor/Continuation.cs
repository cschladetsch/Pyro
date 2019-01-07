using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
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
        public int Ip => _next;

        public Continuation(List<object> code)
        {
            _code = code;
        }

        public string Serialise()
        {
            var str = new StringBuilder();
            Serialise(str, this);
            return str.ToString();
        }

        public string Serialise(StringBuilder str)
        {
            str.Append('{');
            foreach (var elem in Code)
            {
                Serialise(str, elem);
                str.Append(' ');
            }
            str.Append('}');

            return str.ToString();
        }

        void Serialise(StringBuilder str, object obj)
        {
            switch (obj)
            {
                case Continuation cont:
                    Serialise(str);
                    break;
                case EOperation op:
                    str.Append(OpToString(op));
                    break;
                case IRefBase rb:
                    rb.Class.Append(str, rb.BaseValue);
                    break;
                case int n:
                    str.Append(n);
                    break;
                case string s:
                    str.Append('"');
                    str.Append(s);
                    str.Append('"');
                    break;
                case List<object> list:
                    str.Append('[');
                    var sp = ' ';
                    foreach (var elem in list)
                    {
                        Serialise(str, elem);
                        str.Append(sp);
                    }
                    str.Append("] ");
                    break;
                default:
                    str.Append(obj);
                    break;
            }
        }

        private string OpToString(EOperation op)
        {
            return "`" + ((int)op).ToString();
        }

        public object Deserialise(string str)
        {
            return str;
        }

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
        private List<string> _args;
        private readonly List<object> _code;
        private readonly Dictionary<string, object> _scope = new Dictionary<string, object>();

    }
}
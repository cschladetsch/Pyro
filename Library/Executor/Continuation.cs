using System;
using System.Collections.Generic;

namespace Diver.Executor
{
    public class Continuation
    {
        public Continuation()
        {
        }

        public Continuation(IRef<List<object>> code)
        {
            _code = code.Value;
        }

        public bool HasScopeObject(string label)
        {
            return _scope.ContainsValue(label);
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
            var has = _next < _scope.Count;
            next = has ? _code[_next++] : null;
            return has;
        }

        public void Reset()
        {
            _next = 0;
        }

        private int _next;
        private List<object> _code = new List<object>();
        private Dictionary<string, object> _scope = new Dictionary<string, object>();
    }
}
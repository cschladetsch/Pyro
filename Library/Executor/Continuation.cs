﻿using System;
using System.Collections.Generic;

namespace Diver.Exec
{
    /// <summary>
    /// Also known as a co-routine. Can be interrupted mid-execution and later resumed.
    /// </summary>
    public class Continuation
    {
        public List<object> Code { get => _code; set => _code = value; }
        public Dictionary<string, object> Scope { get => _scope; set => _scope = value; }

        public Continuation(List<object> code)
        {
            _code = code;
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
        private List<object> _code;
        private Dictionary<string, object> _scope = new Dictionary<string, object>();
    }
}
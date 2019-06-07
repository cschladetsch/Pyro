using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro.Impl
{
    /// <inheritdoc cref="IClassBase" />
    /// <inheritdoc cref="StructBase" />
    /// <summary>
    /// Common to all Class types used by a Registry.
    /// </summary>
    public class ClassBase
        : StructBase
        , IClassBase
    {
        public int TypeNumber { get; set; }

        private readonly Dictionary<string, ICallable> _callables = new Dictionary<string, ICallable>();

        internal ClassBase(IRegistry reg, Type type)
            : base(reg, type)
        {
        }

        public object Duplicate(object obj)
        {
            return null;
        }

        public ICallable GetCallable(string name)
        {
            return _callables.TryGetValue(name, out var call) ? call : null;
        }

        public void AddCallable(string name, ICallable callable)
        {
            if (_callables.ContainsKey(name))
                throw new Exception("Duplicate callable added to class");

            _callables[name] = callable;
        }

        public virtual void NewRef(Id id, out IRefBase refBase)
        {
            refBase = new RefBase(_registry, this, id);
        }

        protected void AddRefFields(object instance)
        {
            foreach (var field in _fields)
            {

            }
        }

        public IRefBase Create(Id id, object value)
        {
            return new RefBase(_registry, this, id, value);
        }

        public IConstRefBase CreateConst(Id id)
        {
            return new ConstRefBase(_registry, this, id);
        }

        public IConstRefBase CreateConst(Id id, object value)
        {
            return new ConstRefBase(_registry, this, id, value);
        }

        public virtual object NewInstance()
        {
            throw new NotImplementedException();
        }

        public virtual object NewInstance(Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public virtual void ToPiScript(StringBuilder str, object value)
        {
            str.Append(value);
        }
    }
}


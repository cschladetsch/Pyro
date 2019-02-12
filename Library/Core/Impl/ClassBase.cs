using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro.Impl
{
    public class ClassBase
        : StructBase
        , IClassBase
    {
        public int TypeNumber { get; set; }

        internal ClassBase(IRegistry reg, Type type)
            : base(reg, type)
        {
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

        public virtual object NewInstance(Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public virtual void AppendText(StringBuilder str, object value)
        {
            str.Append(value);
        }

        private readonly Dictionary<string, ICallable> _callables = new Dictionary<string, ICallable>();
    }
}

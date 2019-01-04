using System;
using System.Collections.Generic;

namespace Diver.Impl
{
    public class ClassBase : StructBase, IClassBase
    {
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
                throw new Exception("Duplicate callabled added to class");

            _callables[name] = callable;
        }

        public virtual void Create(IRegistry reg, Id id, out IRefBase refBase)
        {
            refBase = new RefBase(reg, this, id);
        }

        public IRefBase Create(IRegistry reg, Id id, object value)
        {
            return new RefBase(reg, this, id, value);
        }

        public IConstRefBase CreateConst(IRegistry reg, Id id)
        {
            return new ConstRefBase(reg, this, id);
        }

        public IConstRefBase CreateConst(IRegistry reg, Id id, object value)
        {
            return new ConstRefBase(reg, this, id, value);
        }

        private readonly Dictionary<string, ICallable> _callables = new Dictionary<string, ICallable>();
    }
}

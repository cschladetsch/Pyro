using System;
using System.Collections.Generic;

namespace Diver.Impl
{
    public class Class<T> : ClassBase, IClass<T>
    {
        internal Class(IRegistry reg) : base(reg, typeof(T))
        {
        }

        public override object NewInstance(Stack<object> stack)
        {
            return null;
        }

        public override void Create(Id id, out IRefBase refBase)
        {
            refBase = new Ref<T>(_registry, this, id);
        }

        public IRef<T> Create(Id id, T value)
        {
            return new Ref<T>(_registry, this, id, value);
        }

        public IConstRef<T> CreateConst(Id id, T value)
        {
            return new ConstRef<T>(_registry, this, id, value);
        }
    }
}

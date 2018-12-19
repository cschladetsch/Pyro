using System;

namespace Diver.Impl
{
    public class Class<T> : ClassBase, IClass<T>
    {
        internal Class(IRegistry reg, Type type) : base(reg, type)
        {
        }

        public new IRef<T> Create(IRegistry reg, Id id)
        {
            return new Ref<T>(reg, this, id);
        }

        public IRef<T> Create(IRegistry reg, Id id, T value)
        {
            return new Ref<T>(reg, this, id, value);
        }

        public new IConstRef<T> CreateConst(IRegistry reg, Id id)
        {
            return new ConstRef<T>(reg, this, id);
        }

        public IConstRef<T> CreateConst(IRegistry reg, Id id, T value)
        {
            return new ConstRef<T>(reg, this, id, value);
        }

    }
}

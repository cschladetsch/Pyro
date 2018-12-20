using System;

namespace Diver.Impl
{
    public class Class<T> : ClassBase, IClass<T>
    {
        internal Class(IRegistry reg) : base(reg, typeof(T))
        {
        }

        public override void Create(IRegistry reg, Id id, out IRefBase refBase)
        {
            refBase = new Ref<T>(reg, this, id);
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

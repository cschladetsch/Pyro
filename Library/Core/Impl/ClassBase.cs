using System;

namespace Diver.Impl
{
    public class ClassBase : StructBase, IClassBase 
    {
        internal ClassBase(IRegistry reg, Type type) 
            : base(reg, type)
        {
        }

        public IRefBase Create(IRegistry reg, Id id)
        {
            return new RefBase(reg, this, id);
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
    }
}

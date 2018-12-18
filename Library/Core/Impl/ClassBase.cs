using System;

namespace Diver.Impl
{
    public class ClassBase : StructBase, IClassBase 
    {
        internal ClassBase(IRegistry reg, Type type) 
            : base(reg, type)
        {
        }

        public IRefBase Create(IRegistry reg, Id id, object value)
        {
            return new RefBase(reg, this, id, value);
        }
    }
}

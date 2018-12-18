using System;

namespace Diver.Impl
{
    public class Class<T> : ClassBase, IClass<T> where T: class, new()
    {
        internal Class(IRegistry reg, Type type) : base(reg, type)
        {
        }
    }
}

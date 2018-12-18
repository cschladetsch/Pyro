using System;

namespace Diver.Impl
{
    public class ClassBase : StructBase, IClassBase 
    {
        internal ClassBase(IRegistry reg, Type type) : base(reg, type)
        {
        }
    }
}

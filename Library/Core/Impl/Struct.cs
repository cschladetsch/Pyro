using System;

namespace Diver.Impl
{
    public class Struct<T> : StructBase, IStruct<T>
    {
        internal Struct(IRegistry reg, Type type) : base(reg, type)
        {
        }
    }
}
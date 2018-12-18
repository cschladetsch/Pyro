using System;

namespace Diver
{
    public interface IClass<T> : IClassBase
    {
        Type InstanceType { get; }
    }
}

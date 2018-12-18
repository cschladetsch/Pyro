using System;

namespace Diver
{
    public interface IConstRefBase
    {
        Id Id { get; }
        IRegistry Registry { get; }
        Type ValueType { get; }
        IClassBase Class { get; }
        bool IsConst { get; }
        object BaseValue { get; }
        object Get();
    }

    public interface IConstRefBase<out T>
    {
        T Value { get; }
    }
}
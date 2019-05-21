using System;

namespace Pyro
{
    /// <summary>
    /// Common to all types that provide const-only access to underlying value.
    /// </summary>
    public interface IConstRefBase
    {
        Id Id { get; }
        IRegistry Registry { get; }
        Type ValueType { get; }
        IClassBase Class { get; }
        bool IsConst { get; }
        object BaseValue { get; }

        T Get<T>();
    }

    public interface IConstRefBase<out T>
    {
        T Value { get; }
    }
}
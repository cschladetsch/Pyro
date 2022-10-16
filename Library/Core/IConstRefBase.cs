namespace Pyro {
    using System;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Common to all types that provide const-only access to underlying value.
    /// </summary>
    public interface IConstRefBase
        : IObject {
        Type ValueType { get; }
        bool IsConst { get; }
        object BaseValue { get; }
        IDictionary<string, object> Scope { get; }
        T Get<T>();
    }

    public interface IConstRefBase<out T> {
        T Value { get; }
    }
}


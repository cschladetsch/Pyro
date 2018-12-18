using System;

namespace Diver
{
    public interface IRegistry
    {
        Guid Guid { get; }

        object Get(Id id);
        void Set(Id id, object value);
        void Set<T>(Id id, T value);

        Id Add(object value);
        IRef<T> Add<T>(T value);
    }
}

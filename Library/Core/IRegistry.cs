using System;

namespace Diver
{
    public interface IRegistry
    {
        Guid Guid { get; }

        object Get(Id id);
        T Get<T>(Id id);

        void Set(Id id, object value);
        IRefBase Add(object value);

        void Set<T>(Id id, T value);
        IRef<T> Add<T>(T value);

        IConstRefBase AddConst(object val);
        IConstRef<T> AddConst<T>(T val);
    }
}

using System;

namespace Diver
{
    public interface IRegistry
    {
        Guid Guid { get; }

        object Get(Id id);
        T Get<T>(Id id);

        void Set(Id id, object value);
        void Set<T>(Id id, T value);

        // TODO: replace with IVal<T> AddVal<T>(T Val);
        IRefBase AddVal<T>(T value);

        IRefBase Add(object value);
        IRef<T> Add<T>(T value) where T : class, new(); 

        IConstRefBase AddConst(Id id, object val);
        IConstRef<T> AddConst<T>(Id id, T val);
    }
}

using System;

namespace Diver
{
    public interface IRegistry
    {
        Guid Guid { get; }

        IRefBase Get(Id id);
        IRef<T> Get<T>(Id id);

        IRefBase Add(object value);
        IRef<T> Add<T>(T value);

        IConstRefBase AddConst(object val);
        IConstRef<T> AddConst<T>(T val);
    }
}

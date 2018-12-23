using System;

namespace Diver
{
    /// <summary>
    /// A mapping of id to object.
    ///
    /// Can be serialised to Json or binary
    /// </summary>
    public interface IRegistry
    {
        Guid Guid { get; }

        IRefBase Get(Id id);
        IRef<T> Get<T>(Id id);

        IRefBase Add(object value);
        IRef<T> Add<T>(T value);
        IRef<T> Add<T>();

        IConstRefBase AddConst(object val);
        IConstRef<T> AddConst<T>(T val);
        IConstRef<T> AddConst<T>();
    }
}

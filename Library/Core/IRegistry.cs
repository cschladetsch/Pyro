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

        bool Register(IClassBase @class);
        IRefBase GetRef(Id id);
        IRef<T> GetRef<T>(Id id);

        T Get<T>(object obj);

        IRefBase Add(object value);
        IRef<T> Add<T>(T value);
        IRef<T> Add<T>();

        IConstRefBase AddConst(object val);
        IConstRef<T> AddConst<T>(T val);
        IConstRef<T> AddConst<T>();
        IClassBase GetClass(Type type);
        IClass<T> GetClass<T>();
    }
}

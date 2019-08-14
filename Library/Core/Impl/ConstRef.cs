namespace Pyro.Impl
{
    /// <inheritdoc cref="IConstRef{T}" />
    /// <summary>
    /// A constant reference to an instance of given type T.
    /// </summary>
    internal class ConstRef<T>
        : ConstRefBase
        , IConstRef<T>
    {
        public T Value => Registry.Get<T>(this);
        public Class<T> TypedClass => Class as Class<T>;

        public ConstRef(IRegistry reg, IClassBase @class, Id id)
            : base(reg, @class, id)
        {
        }

        public ConstRef(IRegistry reg, IClassBase @class, Id id, T value)
            : this(reg, @class, id)
        {
            reg.AddConst(value);
        }
    }
}


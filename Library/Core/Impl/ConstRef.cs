namespace Diver.Impl
{
    internal class ConstRef<T> : ConstRefBase, IConstRef<T>
    {
        public T Value => _registry.Get<T>(_id);

        public Class<T> TypedClass => Class as Class<T>;

        public ConstRef(IRegistry reg, IClass<T> @class, Id id)
            : base(reg, @class, id)
        {
        }

        public ConstRef(IRegistry reg, IClass<T> @class, Id id, T value)
            : this(reg, @class, id)
        {
            reg.AddConst(id, value);
        }

    }
}
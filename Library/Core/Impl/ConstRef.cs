namespace Diver.Impl
{
    internal class ConstRef<T>
        : ConstRefBase, IConstRef<T>
    {
        public T Value => Registry.Get<T>(this);
        public Class<T> TypedClass => Class as Class<T>;

        public ConstRef(IRegistry reg, IClass<T> @class, Id id)
            : base(reg, @class, id)
        {
        }

        public ConstRef(IRegistry reg, IClass<T> @class, Id id, T value)
            : this(reg, @class, id)
        {
            reg.AddConst(value);
        }
    }
}
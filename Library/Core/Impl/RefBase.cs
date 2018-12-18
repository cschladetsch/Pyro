namespace Diver.Impl
{
    internal class RefBase : ConstRefBase, IRefBase
    {
        public new static RefBase None = new RefBase();

        public new bool IsConst => false;
        public void Set(object value)
        {
            _registry.Set(_id, value);
        }

        private RefBase()
        {
        }

        internal RefBase(Id id)
            : base(id)
        {
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id)
            : base(reg, @class, id)
        {
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id, object val)
            : this(reg, @class, id)
        {
            reg.Set(id, val);
        }
    }
}
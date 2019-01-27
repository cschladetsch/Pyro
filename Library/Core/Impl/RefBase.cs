namespace Pryo.Impl
{
    internal class RefBase : ConstRefBase, IRefBase
    {
        //public new static RefBase None = new RefBase(null, null, Id.None);

        public new bool IsConst => false;

        public new object BaseValue
        {
            get => base._baseValue;
            set => base._baseValue = value;
        }

        //private RefBase()
        //{
        //}

        //internal RefBase(Id id)
        //    : base(id)
        //{
        //}

        internal RefBase(IRegistry reg, IClassBase @class, Id id)
            : base(reg, @class, id)
        {
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id, object val)
            : this(reg, @class, id)
        {
            BaseValue = val;
        }
    }
}
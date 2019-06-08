namespace Pyro.Impl
{
    internal class RefBase
        : ConstRefBase
        , IRefBase
    {
        public new bool IsConst => false;

        public new object BaseValue
        {
            get => base._baseValue;
            set => base._baseValue = value;
        }

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

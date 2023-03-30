namespace Pyro.Impl {
    internal class RefBase
        : ConstRefBase
            , IRefBase {
        internal RefBase(IRegistry reg, IClassBase @class, Id id)
            : base(reg, @class, id) {
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id, object val)
            : this(reg, @class, id) {
            BaseValue = val;
        }

        public new bool IsConst => false;

        public new object BaseValue {
            get => _baseValue;
            set => _baseValue = value;
        }
    }
}
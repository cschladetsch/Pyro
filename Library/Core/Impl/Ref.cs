using System;

namespace Pyro.Impl {
    /// <inheritdoc cref="IRef{T}" />
    internal class Ref<T>
        : ConstRef<T>
        , IRef<T> {

        public new T Value
        {
            get => (T)BaseValue;
            set => BaseValue = value;
        }

        public new object BaseValue
        {
            get => _baseValue;
            set => _baseValue = value;
        }

        public Ref(IRegistry reg, IClassBase @class, Id id)
            : base(reg, @class, id) {
        }

        public Ref(IRegistry reg, IClassBase class1, Id id, T value)
            : base(reg, class1, id, value) {
        }
    }
}

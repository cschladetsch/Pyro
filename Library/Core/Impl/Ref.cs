using System;

namespace Pyro.Impl
{
    internal class Ref<T> : ConstRef<T>, IRef<T>
    {
        public Ref(IRegistry reg, IClass<T> @class, Id id)
            : base(reg, @class, id)
        {
        }

        public Ref(IRegistry reg, IClass<T> class1, Id id, T value)
            : base(reg, class1, id, value)
        {
        }

        public new T Value
        {
            get => (T) BaseValue;
            set => BaseValue = value;
        }

        public new object BaseValue
        {
            get => _baseValue;
            set => _baseValue = value;
        }

        public void Set(object value)
        {
            throw new NotImplementedException();
        }
    }
}
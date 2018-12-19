using System;

namespace Diver.Impl
{
    /// <summary>
    /// Common implementation behind all type-specific objects in the Dive system.
    /// </summary>
    internal class ConstRefBase : IConstRefBase
    {
        public static ConstRefBase None = new ConstRefBase(Diver.Id.None);
        public Id Id => _id;
        public IRegistry Registry => _registry;
        public Type ValueType => BaseValue?.GetType();
        public IClassBase Class => _classBase;
        public bool IsConst => true;
        public object BaseValue => _baseValue;

        public T Get<T>()
        {
            return (T) _baseValue;
        }

        public ConstRefBase(Id id)
        {
            _id = id;
        }

        public ConstRefBase(IRegistry reg, IClassBase klass, Id id)
        {
            _id = id;
            _registry = reg;
            _classBase = klass;
        }

        public ConstRefBase(IRegistry reg, IClassBase @class, Id id, object val)
            : this(reg, @class, id)
        {
            reg.AddConst(val);
        }

        protected ConstRefBase()
        {
        }
        protected readonly Id _id;
        protected object _baseValue;
        protected readonly IRegistry _registry;
        protected readonly IClassBase _classBase;

    }
}

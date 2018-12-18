using System;

namespace Diver.Impl
{
    /// <summary>
    /// Common implementation behind all type-specific objects in the Dive system.
    /// </summary>
    public class ConstRefBase : IConstRefBase
    {
        public static ConstRefBase None = new ConstRefBase(Diver.Id.None);
        public Id Id => _id;
        public IRegistry Registry => _registry;
        public Type ValueType => BaseValue?.GetType();
        public IClassBase Class => _classBase;
        public bool IsConst => true;

        public object Get()
        {
            return _registry.Get(_id);
        }

        public object BaseValue
        {
            get => _registry.Get(_id);
            set => _registry.Set(_id, value);
        }

        internal ConstRefBase()
        {
        }

        internal ConstRefBase(Id id)
        {
            _id = id;
        }

        internal ConstRefBase(IRegistry reg, IClassBase @class, Id id)
        {
            _registry = reg;
            _id = id;
            _classBase = @class;
        }

        internal ConstRefBase(IRegistry reg, IClassBase @class, Id id, object val)
            : this(reg, @class, id)
        {
            reg.AddConst(id, val);
        }

        protected readonly IRegistry _registry;
        protected readonly Id _id;
        protected readonly IClassBase _classBase;
    }
}

using System;

namespace Diver.Impl
{
    /// <summary>
    /// Common implementation behind all type-specific objects in the Dive system.
    /// </summary>
    public class RefBase : IRefBase
    {
        public Id Id => _id;
        public IRegistry Registry => _registry;
        public Type ValueType => BaseValue?.GetType();
        public IClassBase Class => _classBase;
        public void Set(object value)
        {
            _registry.Set(_id, value);
        }

        public object Get()
        {
            return _registry.Get(_id);
        }

        public object BaseValue
        {
            get => _registry.Get(_id);
            set => _registry.Set(_id, value);
        }

        internal RefBase()
        {
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id)
        {
            _registry = reg;
            _id = id;
            _classBase = @class;
        }

        internal RefBase(IRegistry reg, IClassBase @class, Id id, object val = null)
            : this(reg, @class, id)
        {
            BaseValue = val;
        }

        private readonly IRegistry _registry;
        private readonly Id _id;
        private readonly IClassBase _classBase;
    }
}

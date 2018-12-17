using System;
using System.ServiceModel.Configuration;

namespace Diver.Impl
{
    /// <summary>
    /// Common implementation behind all type-specific objects in the Dive system.
    /// </summary>
    public class ObjectBase : IObjectBase
    {
        public Id Id => _id;
        public IRegistry Registry => _registry;
        public object BaseValue
        {
            get => _value;
            set => _classBase.SetValue(this, value);
        }
        public Type ValueType => BaseValue?.GetType();
        public IClassBase Class => _classBase;

        internal ObjectBase()
        {
        }

        internal ObjectBase(IRegistry reg, Id id, IClassBase @class, object val = null)
        {
            _registry = reg;
            _id = id;
            _classBase = @class;
            _value = val;
        }

        private readonly object _value;
        private readonly Id _id;
        private readonly IRegistry _registry;
        private readonly IClassBase _classBase;
    }
}

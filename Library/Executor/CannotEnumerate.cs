using System;

namespace Pyro.Exec
{
    internal class CannotEnumerate
        : Exception
    {
        private readonly object _obj;

        public CannotEnumerate(object obj)
            => _obj = obj;

        public override string ToString()
            => $"Cannot enumerate over {_obj.GetType().Name}";
    }
}


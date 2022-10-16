﻿namespace Pyro.Exec
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Cannot enumerate over values in given object.
    /// </summary>
    public class CannotEnumerate
        : Exception
    {
        private readonly object _obj;

        public CannotEnumerate(object obj)
            => _obj = obj;

        public override string ToString()
            => $"Cannot enumerate over {_obj?.GetType().Name}";
    }
}


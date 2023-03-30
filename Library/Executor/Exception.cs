﻿using System;

namespace Pyro.Exec {
    /// <inheritdoc />
    /// <summary>
    ///     Cannot enumerate over values in given object.
    /// </summary>
    public class CannotEnumerate
        : Exception {
        private readonly object _obj;

        public CannotEnumerate(object obj) {
            _obj = obj;
        }

        public override string ToString() {
            return $@"Cannot enumerate over {_obj?.GetType().Name}";
        }
    }
}
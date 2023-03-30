using System;

namespace Pyro.Exec {
    /// <summary>
    ///     DOC
    /// </summary>
    public class CannotConvertException
        : Exception {
        public object Object;
        public Type TargetType;

        public CannotConvertException(object obj, Type type) {
            Object = obj;
            TargetType = type;
        }

        public override string ToString() {
            return $"Couldn't convert {Object} to type {TargetType.Name}";
        }
    }
}
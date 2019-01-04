using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Diver.Exec
{
    [Serializable]
    internal class CannotEnumerate : Exception
    {
        private object obj;

        public CannotEnumerate(object obj)
        {
            this.obj = obj;
        }

        public override string ToString()
        {
            return $"Cannot enumerate over {obj.GetType().Name}";
        }
    }
}
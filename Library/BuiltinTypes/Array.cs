using System.Collections.Generic;

namespace BuiltinTypes
{
    public class Array
    {
        public List<object> Objects => _objects;
        public int? Count => _objects?.Count;
#pragma warning disable CS0649 // Field 'Array._objects' is never assigned to, and will always have its default value null
        private List<object> _objects;
#pragma warning restore CS0649 // Field 'Array._objects' is never assigned to, and will always have its default value null
    }
}

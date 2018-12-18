using System.Collections.Generic;

namespace BuiltinTypes
{
    public class Array
    {
        public List<object> Objects => _objects;
        public int? Count => _objects?.Count;
        private List<object> _objects;
    }
}

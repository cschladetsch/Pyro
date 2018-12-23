using System.Collections.Generic;
using System.Security;

namespace BuiltinTypes
{
    public class Array
    {
        public IList<object> Objects => _objects;
        public int Count => _objects.Count;
        private readonly List<object> _objects = new List<object>();
    }

    public class Map
    {
        public IDictionary<object, object> Map => _map;

        private Dictionary<object, object> _map = new Dictionary<object, object>(EqualObject);
    }
}

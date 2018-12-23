using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.ServiceModel.Configuration;

namespace Diver
{
    public class Array
    {
        public IList<object> Objects => _objects;
        public int Count => _objects.Count;
        private readonly List<object> _objects = new List<object>();
    }

    public class Map
    {
        public IDictionary<object, object> Contents => _map;

        private readonly IComparer<object> _comparer = new EqualityComp();
        private readonly Dictionary<object, object> _map = new Dictionary<object, object>();
    }

    internal class EqualityComp : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return Operations.Compare(x, y);
        }
    }
}

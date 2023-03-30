using System.Collections.Generic;

namespace Pyro.BuiltinTypes {
    public class Map
        : Dictionary<object, object> {
        public Map()
            : base(new CustomMapComparer()) {
        }
    }
}
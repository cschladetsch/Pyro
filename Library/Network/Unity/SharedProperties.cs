using UnityEngine;

namespace Pyro.Network.Unity {
    using System.Collections.Generic;
    using System.Reflection;

    public class SharedProperties
        : Common {
        public GameObject Target;
        public IList<PropertyInfo> Properties { get; }
        public IList<FieldInfo> Fields { get; }
    }
}
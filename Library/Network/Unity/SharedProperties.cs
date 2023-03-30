using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Pyro.Network.Unity {
    public class SharedProperties
        : Common {
        public GameObject Target;
        public IList<PropertyInfo> Properties { get; }
        public IList<FieldInfo> Fields { get; }
    }
}
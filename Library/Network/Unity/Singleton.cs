using UnityEngine;

namespace Pyro.Network.Unity {
    public class Singleton<T>
        : MonoBehaviour
        where T : class {
        public T Instance => null;
    }
}
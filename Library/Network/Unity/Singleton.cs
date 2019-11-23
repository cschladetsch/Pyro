namespace Pyro.Network.Unity
{
    using UnityEngine;
    
    public class Singleton<T>
        : MonoBehaviour
        where T : class
    {
        public T Instance
        {
            get { return null; }
        }
    }
}
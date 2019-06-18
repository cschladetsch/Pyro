using System.Collections.Generic;

namespace Pyro.Unity3d
{
    public class Transform : Component
    {
        public GameObject GameObject;
        public Transform Parent;
        public List<Transform> Children = new List<Transform>();
        public Vector3 Position;
        public Vector3 LocalScale;
        public Quaternion Rotation;
    }
}

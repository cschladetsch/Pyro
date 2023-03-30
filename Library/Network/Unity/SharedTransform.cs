using UnityEngine;

namespace Pyro.Network.Unity {
    /// <summary>
    ///     An Object Transform that is shared across the network.
    /// </summary>
    public class SharedTransform
        : Common {
        public Vector3 Position;
        public Quaternion Rotation;
        public float MinDeltaPosition = 0.1f;
        public float MinDeltaRotation = 0.1f;

        private void Update() {
        }
    }
}
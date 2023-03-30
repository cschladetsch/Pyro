using Flow;

namespace Pyro.Network.Unity {
    public class NetworkKernel
        : Singleton<NetworkKernel> {
        public IKernel Kernel;
        public IPeer Peer;

        private void Awake() {
            DontDestroyOnLoad(gameObject);

            Kernel = Flow.Create.Kernel();
        }

        private void Update() {
            Kernel.Step();
        }
    }
}
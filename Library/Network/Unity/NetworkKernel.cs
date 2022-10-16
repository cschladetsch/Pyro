namespace Pyro.Network.Unity
{
    using Flow;

    public class NetworkKernel
        : Singleton<NetworkKernel>
    {
        public IKernel Kernel;
        public IPeer Peer;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Kernel = Create.Kernel();
        }

        private void Update()
        {
            Kernel.Step();
        }
    }
}
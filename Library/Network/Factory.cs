using Pyro.Network.Impl;

namespace Pyro.Network {
    /// <summary>
    ///     Factory for network objects.
    /// </summary>
    public static class Factory {
        public static IPeer NewPeer(IDomain domain, int port) {
            return new Peer(domain, port);
        }

        public static void RegisterTypes(IRegistry registry) {
            Peer.Register(registry);
        }
    }
}
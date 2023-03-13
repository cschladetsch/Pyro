namespace Pyro.Network {
    /// <summary>
    /// Factory for network objects.
    /// </summary>
    
    public static class Factory {
        public static IPeer NewPeer(IDomain domain, int port) {
            return new Impl.Peer(domain, port);
        }
        
        public static void RegisterTypes(IRegistry registry) {
            Impl.Peer.Register(registry);
        }
    }
}


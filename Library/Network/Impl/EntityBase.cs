namespace Pyro.Network.Impl {
    /// <summary>
    /// Common to untyped Proxies and Agents
    /// </summary>
    public class EntityBase
        : INetworkEntity {
        public NetId NetId => _netId;

        private Pyro.Network.NetId _netId;

        public EntityBase(NetId sourceNetId) {
            _netId = sourceNetId;
        }
    }
}
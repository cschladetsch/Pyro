namespace Pyro.Network.Impl {
    /// <summary>
    ///     Common to untyped Proxies and Agents
    /// </summary>
    public class EntityBase
        : INetworkEntity {
        public EntityBase(NetId sourceNetId) {
            NetId = sourceNetId;
        }

        public NetId NetId { get; }
    }
}
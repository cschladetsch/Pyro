namespace Pyro.Network.Impl {
    /// <summary>
    ///     Common to Typed Proxies and Agents
    public class EntityCommon<T>
        : EntityBase {
        private IRef<T> _ref;

        public EntityCommon(NetId sourceNetId, IRef<T> obj)
            : base(sourceNetId) {
            _ref = obj;
        }
    }
}
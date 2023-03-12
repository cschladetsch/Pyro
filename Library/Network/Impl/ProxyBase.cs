namespace Pyro.Network.Impl {
    /// <summary>
    /// Common to all typed Proxies
    /// </summary>
    public class ProxyBase<T>
        : EntityCommon<T> {
        public ProxyBase(NetId sourceNetId, IRef<T> obj) : base(sourceNetId, obj) {
        }
    }
}
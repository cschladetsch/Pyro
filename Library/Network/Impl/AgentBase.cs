namespace Pyro.Network.Impl {
    /// <summary>
    ///     Common to all typed Agents
    /// </summary>
    public class AgentBase<T>
        : EntityCommon<T> {
        public AgentBase(NetId sourceNetId, IRef<T> obj)
            : base(sourceNetId, obj) {
        }
    }
}
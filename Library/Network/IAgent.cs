namespace Pyro.Network {
    public interface IAgent<out TInterface>
        : IAgentBase
        where TInterface : IReflected {
        TInterface Servant { get; }
    }
}
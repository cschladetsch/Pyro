using System;

namespace Diver.Network
{
    public class Peer
    {
    }

    public class NetId
    {
        private Guid _registy;
        private Id _id;
    }

    public interface INetworkClass<T> : IClassBase
    {
        IAgent<T> NewAgent();
        IProxy<T> NewProxy();
    }

    public interface IProxy<T> : IProxyBase
    {
    }

    public interface IProxyBase
    {
    }

    public interface IAgent<T> : IAgentBase
    {
    }

    public interface IAgentBase
    {
    }
}

using Flow;
using Pyro;
using Pyro.Network;
using System.Collections.Generic;

public interface INetworkEntity {
    Pyro.Network.NetId NetId { get; } 
}

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

// TODO: IConstRe<T>

/// <summary>
/// Common to Typed Proxies and Agents
public class EntityCommon<T>
    : EntityBase {

    private IRef<T> _ref;

    public EntityCommon(NetId sourceNetId, IRef<T> obj)
        : base(sourceNetId) {
        _ref = obj;
    }
}

/// <summary>
/// Common to all typed Proxies
/// </summary>
/// <typeparam name="T"></typeparam>
public class ProxyBase<T>
    : EntityCommon<T> {
    public ProxyBase(NetId sourceNetId, IRef<T> obj) : base(sourceNetId, obj) {
    }
}

/// <summary>
/// Common to all typed Agents
/// </summary>
public class AgentBase<T>
    : EntityCommon<T> {
    public AgentBase(NetId sourceNetId, IRef<T> obj): base(sourceNetId, obj) {
    }
}

struct Null { };


public class NetEvent<ReturnValue, Class>
    {
    public ReturnValue Invoke(params object[] args) {
        return default;
    }
}

public class NetEvent<ReturnValue, Class, T0> {
}

public class NetEvent<ReturnValue, Class, T0, T1> {
}


//================================================================

namespace Foo.Agent {
    public partial class BarAgent<TBarProxy> {
        protected List<TBarProxy> _Proxies = new List<TBarProxy>();
    }
}

namespace Foo.Proxy {
    public partial class BarProxy {
        public BarProxy(NetId sourceNetId, IRef<BarProxy> obj) : base(sourceNetId, obj) {
        }
    }
}

namespace Foo.Agent {
    public interface IAgentCommon {
    }

    public interface IAgentBase<IAgent>
        : IAgentCommon {

    }
}

//----------------------------------------------------

/// <summary>
/// Many subscribers can subscribe to the one network event.
/// </summary>
/// <typeparam name="BarAgentSomeEventHandler"></typeparam>
public class NetworkEvent<NetworkEventHandler> {
    private List<INetworkEntity> _subscribers;

}

namespace Foo.Agent.Impl {

    public interface IBarAgent
        : IAgentBase<IBarAgent> {

    }
    public partial class BarAgent
        : AgentBase<BarAgent> { 

        public delegate void BarAgentSomeEventHandler(int num, string str);
        NetworkEvent<BarAgentSomeEventHandler> SomeEvent;

        public BarAgent(NetId sourceNetId, IRef<BarAgent> obj) : base(sourceNetId, obj) {
        }

        string Name { get; }
        float Age {  set; get; }

        int Sum(int a, int b) {
            return a + b;
        }

        Null Call() {
            return default;
        }
    }
}

namespace Foo.Proxy {
    using Foo.Agent.Impl;

    public partial class BarProxy
        : ProxyBase<BarProxy> {

        public delegate void BarProxySomeEventHandler(IBarAgent sender, int num, string str);
        event BarProxySomeEventHandler SomeEvent;

        /// <summary>
        /// Can have many proxies to one Agent.
        /// </summary>
        IBarAgent _agent;

        IFuture<string> Name { get; }
        IFuture<float> Age {  set; get; }

        IFuture<int> Sum(int a, int b) {
            return null;
        }

        IFuture<Null> Call() {
            return null;
        }
    }
}


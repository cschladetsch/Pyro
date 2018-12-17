using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diver.Network
{
    public class Peer
    {
    }

    public class Id
    {
        private Guid _registy;
        private int _id;
    }

    public interface IClass<T> : IClassBase
    {
        IAgent<T> NewAgent();
        IProxy<T> NewProxy();
    }

    public interface IClassBase
    {
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

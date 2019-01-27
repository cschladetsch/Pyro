using System.Net.Sockets;
using Pryo;
using Pyro.ExecutionContext;

namespace Pyro.Network
{
    public interface INetCommon : IProcess
    {
        Context Context { get; }
        Socket Socket { get; }
    }
}
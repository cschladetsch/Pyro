using System.Net.Sockets;

namespace Pyro.Network
{
    using ExecutionContext;

    /// <summary>
    /// Common interface to clients and servers.
    /// </summary>
    public interface INetCommon
        : IProcess
    {
        Context Context { get; }
        Socket Socket { get; }
    }
}

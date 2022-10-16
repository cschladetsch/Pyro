namespace Pyro.Network {
    using ExecutionContext;
    using Flow;
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Common interface to clients and servers.
    /// </summary>
    public interface INetCommon
        : IProcess {
        Context Context { get; }
        Socket Socket { get; }

        IFuture<DateTime> Ping();
    }
}

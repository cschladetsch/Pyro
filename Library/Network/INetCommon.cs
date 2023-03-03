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
        ExecutionContext ExecutionContext { get; }
        Socket Socket { get; }

        IFuture<DateTime> Ping();
    }
}

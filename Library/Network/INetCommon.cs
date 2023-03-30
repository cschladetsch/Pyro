using System;
using System.Net.Sockets;
using Flow;

namespace Pyro.Network {
    /// <summary>
    ///     Common interface to clients and servers.
    /// </summary>
    public interface INetCommon
        : IProcess {
        ExecutionContext.ExecutionContext ExecutionContext { get; }
        Socket Socket { get; }

        IFuture<DateTime> Ping();
    }
}
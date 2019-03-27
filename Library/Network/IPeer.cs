using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flow;
using Pyro.Exec;

namespace Pyro.Network
{
    public delegate void ReceivedResponseHandler(IServer server, IClient client, string text);
    public delegate void ConnectedHandler(IPeer peer, IClient client);

    /// <summary>
    /// A Peer listens to incoming connections, and can connect to other peers.
    /// </summary>
    public interface IPeer : IProcess
    {
        IServer Local { get; }
        IClient Remote { get; }
        IList<IClient> Clients { get; }
        string LocalHostName { get; }

        event ReceivedResponseHandler OnReceivedResponse;
        event ConnectedHandler OnConnected;

        bool Start();
        bool Connect(string hostName, int port);
        bool Enter(IClient client);
        bool Execute(Continuation continuation);
        bool Execute(string pi);
        void Leave();
        void Stop();

        /// <summary>
        /// Make a new local Agent
        /// </summary>
        TIAgent NewAgent<TIAgent>();

        /// <summary>
        /// Make a local proxy to a remote Agent
        /// </summary>
        IFuture<TIProxy> NewProxy<TIProxy>(Guid agentNetId);

        IFuture<TR> RemoteCall<TR>(NetId agentId, string methodName);
        IFuture<TR> RemoteCall<TR, T0>(NetId agentId, string methodName, T0 t0);
        IFuture<TR> RemoteCall<TR, T0, T1>(NetId agentId, string methodName, T0 t0, T1 t1);
    }
}

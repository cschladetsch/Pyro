using System.Collections.Generic;
using Pryo;
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
    }
}

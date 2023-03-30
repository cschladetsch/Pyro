using System.Net.Sockets;

namespace Pyro.Network {
    public delegate void ClientReceivedHandler(IClient client, Socket server);

    /// <inheritdoc />
    /// <summary>
    ///     The Client interface.
    /// </summary>
    public interface IClient
        : INetCommon {
        string HostName { get; }
        int HostPort { get; }
        event ClientReceivedHandler OnReceived;

        void GetLatest();
        bool Continue(string piScript);
        bool ContinueRho(string rhoScript);
        void Close();
    }
}
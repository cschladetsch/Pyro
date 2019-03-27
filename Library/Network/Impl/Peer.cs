using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Flow;
using Pyro.Exec;

namespace Pyro.Network.Impl
{
    public class Peer
        : NetworkConsoleWriter
        , IPeer
    {
        public string LocalHostName => GetLocalHostname();
        public event ReceivedResponseHandler OnReceivedResponse;
        public event ConnectedHandler OnConnected;
        public IList<object> Stack { get; }

        public IList<IClient> Clients => _clients.Cast<IClient>().ToList();
        public IServer Local => _server;
        public IClient Remote => _remote;
        public string HostName => GetHostName();
        public int HostPort => GetHostPort();

        private Server _server;
        private readonly List<Client> _clients = new List<Client>();
        private IClient _remote;

        public Peer(int listenPort)
        {
            _server = new Server(this, listenPort);
            _server.RecievedResponse += (server, client, text) =>
            {
                OnReceivedResponse?.Invoke(server, client, text);
            };
        }

        public bool SelfHost(int port)
        {
            if (!Connect(GetLocalHostname(), port))
                return Error("Couldn't connect to localhost");

            // unsure if truly needed, but this Sleep is to give a little time for local
            // client to connect to local server via Tcp
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(.2));

            return Enter(Clients[0]) || Error("Couldn't shell to localhost");
        }

        public IClient GetClient(Socket sender)
        {
            return _clients.FirstOrDefault(c => c.Socket == sender);
        }

        public string GetHostName()
        {
            return GetRemoteEndPoint()?.Address.ToString();
        }

        private int GetHostPort()
        {
            return GetRemoteEndPoint()?.Port ?? 0;
        }

        public bool Start()
        {
            return !_server.Start() ? Fail("Couldn't start server") : SelfHost(_server.ListenPort);
        }

        public bool EnterRemote(Client client)
        {
            //WriteLine($"Remoting to {client.Socket.RemoteEndPoint}");
            if (!_clients.Contains(client))
                return false;
            _remote = client;
            return true;
        }

        public void Leave()
        {
            // TODO: maybe? keep a stack of remotes to pop from
            _remote = null;
        }

        public bool Connect(string hostName, int port)
        {
            var client = new Client(this);
            if (!client.Connect(hostName, port)) 
                return false;

            _clients.Add(client);
            return true;
        }

        public bool Enter(IClient client)
        {
            if (client == null)
                return Fail("Null client");
            if (client.Socket == null || !client.Socket.Connected)
                return Fail("Client not connected");

            _remote = client;
            return true;
        }

        public void Stop()
        {
            WriteLine($"Closing {_clients.Count} connections");
            foreach (var client in _clients)
                client.Close();

            _clients.Clear();
            _server.Stop();
            _server = null;
        }

        public TIAgent NewAgent<TIAgent>()
        {
            throw new NotImplementedException();
        }

        public IFuture<TIProxy> NewProxy<TIProxy>(Guid agentNetId)
        {
            throw new NotImplementedException();
        }

        public IFuture<TR> RemoteCall<TR>(NetId agentId, string methodName)
        {
            throw new NotImplementedException();
        }

        public IFuture<TR> RemoteCall<TR, T0>(NetId agentId, string methodName, T0 t0)
        {
            throw new NotImplementedException();
        }

        public IFuture<TR> RemoteCall<TR, T0, T1>(NetId agentId, string methodName, T0 t0, T1 t1)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
        }

        public void NewConnection(Socket socket)
        {
            //WriteLine($"Connected to {socket.RemoteEndPoint}");
            var address = socket.RemoteEndPoint as IPEndPoint;
            if (address == null)
            {
                Error($"{socket} is not an IPEndPoint");
                return;
            }

            foreach (var client in _clients)
            {
                if (client.HostName != address.Address.ToString()) 
                    continue;
                //client.CompleteConnect(socket);
                OnConnected?.Invoke(this, client);
                return;
            }

            Error($"Failed to find client for {socket.RemoteEndPoint}");
        }

        public bool Execute(string script)
        {
            return _remote?.Continue(script) ?? Fail("Not connected");
        }

        public string GetLocalHostname()
        {
            var address = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return address?.ToString() ?? "localhost";
        }

        public bool Execute(Continuation continuation)
        {
            return _remote?.Continue(continuation) ?? Fail("Not connected");
        }

        public bool EnterRemoteAt(int index)
        {
            return index >= _clients.Count ? Fail("No such client id") : EnterRemote(_clients[index]);
        }

        private IPEndPoint GetRemoteEndPoint()
        {
            var socket = Remote?.Socket;
            return socket?.RemoteEndPoint as IPEndPoint;
        }

    }
}
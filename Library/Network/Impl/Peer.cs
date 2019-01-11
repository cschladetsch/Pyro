using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Diver.Exec;

namespace Diver.Network.Impl
{
    public class Peer : NetworkConsoleWriter, IPeer
    {
        public event ReceivedResponseHandler OnReceivedResponse;
        public event ConnectedHandler OnConnected;
        public IList<object> Stack { get; }

        public IList<IClient> Clients => _clients.Cast<IClient>().ToList();
        public IServer Local => _server;
        public IClient Remote => _remote;
        public string HostName => GetHostName();
        public int HostPort => GetHostPort();

        public Peer(int listenPort)
        {
            _server = new Server(this, listenPort);
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
            return _server.Start();
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
            return Fail("Not implemented");
        }

        //public bool Disconnect(Socket socket)
        //{
        //    if (!_connections.Contains(socket))
        //        return Fail($"Not connected to {socket.RemoteEndPoint}");

        //    socket.Close();
        //    _connections.Remove(socket);
        //    return true;
        //}

        public void Stop()
        {
            WriteLine($"Closing {_clients.Count} connections");
            foreach (var client in _clients)
                client.Close();

            _clients.Clear();
            _server.Stop();
            _server = null;
        }

        public void Update()
        {
        }

        public void NewConnection(Socket socket)
        {
            WriteLine($"Connected to {socket.RemoteEndPoint}");
            //_connections.Add(socket);
            var address = socket.RemoteEndPoint as IPEndPoint;
            foreach (var client in _clients)
            {
                if (client.HostName == address.Address.ToString())
                {
                    client.CompleteConnect(socket);
                }
            }
        }

        public bool Execute(string script)
        {
            return _server.Execute(script);
        }

        public string GetLocalHostname()
        {
            var address = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return address?.ToString() ?? "localhost";
        }

        public bool Continue(Continuation continuation)
        {
            return _remote?.Continue(continuation) ?? Fail("Not connected");
        }

        private IPEndPoint GetRemoteEndPoint()
        {
            var socket = Remote?.Socket;
            return socket?.RemoteEndPoint as IPEndPoint;
        }

        private IServer _server;
        private readonly List<Client> _clients = new List<Client>();
        private IClient _remote;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Flow;

namespace Pyro.Network.Impl {
    /// <inheritdoc cref="IPeer" />
    /// <summary>
    ///     A network peer. Contains a client and a server.
    /// </summary>
    public class Peer
        : Server
            , IPeer {
        private readonly List<IClient> _clients = new List<IClient>();

        //        public event OnWriteDelegate OnWrite
        //        {
        //            add
        //            {
        //                base.OnWrite += value;
        //            }
        //            remove
        //            {
        //                base.OnWrite -= value;
        //            }
        //        }

        public Peer(IDomain domain, int listenPort)
            //FIX
            : base(null, listenPort) {
            Domain = domain;
            StartServer(listenPort);
        }

        public string HostName => GetHostName();
        public int HostPort => GetHostPort();
        public string LocalHostName => GetLocalHostname();

        public event MessageHandler OnReceivedRequest;

        // We connected to a new remote server (and made a new local client for it)
        public event ConnectedHandler OnConnected;

        // a local client (and there could be many different local clients) received a response.
        public event MessageHandler OnReceivedResponse;

        public IList<IClient> Clients => _clients.ToList();
        public IDomain Domain { get; }
        public IServer Local { get; private set; }

        public IClient Remote { get; private set; }

        public bool SelfHost() {
            return !Local.Start() ? Fail("Couldn't start server") : SelfHost(Local.ListenPort);
        }

        public bool Enter(int index) {
            return index >= _clients.Count ? Fail($"No such client id={index}") : EnterRemote(_clients[index]);
        }

        public bool Execute(string script) {
            return Remote?.Continue(script) ?? Fail("Not connected");
        }

        public void Leave() {
            if (Remote == _clients[0]) {
                Error("Cannot leave self");
                return;
            }

            // Go back to self-hosting.
            Remote = _clients[0];
        }

        public bool Connect(string hostName, int port) {
            var client = new Client(this);
            if (!client.Connect(hostName, port)) {
                return false;
            }

            _clients.Add(client);
            return true;
        }

        public bool ShowStack(int i) {
            if (i >= _clients.Count) {
                return Error("Invalid client number.");
            }

            var client = _clients[i];

            Console.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            // make a copy as it could be changed by another call while we're iterating over data stack
            var results = client.ExecutionContext.Executor.DataStack.ToList();
            var reg = client.ExecutionContext.Registry;
            var n = results.Count - 1;
            foreach (var result in results)
                str.AppendLine($"{n--}: {reg.ToPiScript(result)}");

            Console.Write(str.ToString());
            return true;
        }

        public bool SendPiToClient(int n, string piScript) {
            return n >= _clients.Count ? Error("Invalid client number.") : _clients[n].Continue(piScript);
        }

        public bool EnterClient(IClient client) {
            if (client == null) {
                return Fail("Null client");
            }

            if (client.Socket == null || !client.Socket.Connected) {
                return Fail("Client not connected");
            }

            Remote = client;
            OnConnected?.Invoke(this, client);
            return true;
        }

        public IFuture<TR> RemoteCall<TR>(NetId agentId, string methodName) {
            throw new NotImplementedException();
        }

        public IFuture<TR> RemoteCall<TR, T0>(NetId agentId, string methodName, T0 t0) {
            throw new NotImplementedException();
        }

        public IFuture<TR> RemoteCall<TR, T0, T1>(NetId agentId, string methodName, T0 t0, T1 t1) {
            throw new NotImplementedException();
        }

        public override Socket Socket { get; set; }

        public override string ToString() {
            var text = $"Peer: {Clients.Count} clients, ";
            if (Local != null) {
                text += $"{Local}";
            }
            else {
                text += "no server";
            }

            return $"\"{text}\"";
        }

        public static void Register(IRegistry reg) {
            reg.Register(new ClassBuilder<Peer>(reg)
                .Methods
                .Add<string, int, bool>("Connect", (q, s, p) => q.Connect(s, p))
                .Add<int>("StartServer", (q, s) => q.StartServer(s))
                .Add<int, bool>("Remote", (q, s) => q.Enter(s))
                .Add("Leave", q => q.Leave())
                .Class);
        }

        public IClient GetClient(Socket sender) {
            return _clients.FirstOrDefault(c => c.Socket == sender);
        }

        public bool Listen() {
            return Local.Start();
        }

        public void Received(Socket socket, string text) {
            OnReceivedResponse?.Invoke(this, FindClient(socket), text);
        }


        public TIAgent NewAgent<TIAgent>() {
            // TODO NEXT
            //var agent = _server.NewAgent<TIAgent>();
            //agent.Bind(this);
            //
            //return null;
            throw new NotImplementedException();
        }

        public IFuture<TIProxy> NewProxy<TIProxy>(Guid agentNetId) {
            throw new NotImplementedException();
        }

        public void SwitchClient(int n) {
            if (n >= _clients.Count) {
                Error($"Invalid client number {n}");
                return;
            }

            Remote = _clients[n];
        }

        public void NewConnection(Socket socket) {
            var client = new Client(this) { Socket = socket };
            _clients.Add(client);
            OnConnected?.Invoke(this, client);

            WriteLine($"Connected to {socket.RemoteEndPoint}");
        }

        private static string GetLocalHostname() {
            var address = Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return address?.ToString() ?? "localhost";
        }

        public void ShowEndPoints() {
            foreach (var client in _clients)
                WriteLine($"{client.Socket.LocalEndPoint} -> {client.Socket.RemoteEndPoint}");
        }

        public void NewServerConnection(Socket socket) {
            //WriteLine($"NewServerConn: {socket.RemoteEndPoint}");
            var client = new Client(this) { Socket = socket };
            _clients.Add(client);
        }

        private void StartServer(int listenPort) {
            Local = new Server(this, listenPort);
            //FIX
            // _server.ReceivedRequest += (client, text) => {
            //     OnReceivedRequest?.Invoke(this, client, text);
            // };
        }

        private string GetHostName() {
            return GetRemoteEndPoint()?.Address.ToString();
        }

        private IPEndPoint GetRemoteEndPoint() {
            return Remote?.Socket?.RemoteEndPoint as IPEndPoint;
        }

        private int GetHostPort() {
            return GetRemoteEndPoint()?.Port ?? 0;
        }

        private bool EnterRemote(IClient client) {
            if (client.Socket == null) {
                return false;
            }

            WriteLine($"Remoting into {client.Socket.RemoteEndPoint}");
            if (!_clients.Contains(client)) {
                return false;
            }

            Remote = client;
            return true;
        }

        /// <summary>
        ///     Connect to local loopback address.
        /// </summary>
        /// <param name="port">The port to connect to.</param>
        /// <returns>True if connection made,</returns>
        private bool SelfHost(int port) {
            if (!Connect(GetLocalHostname(), port)) {
                return Error("Couldn't connect to localhost");
            }

            // This Sleep is to give a little time for local
            // client to connect to local server via loopback Tcp.
            Thread.Sleep(TimeSpan.FromMilliseconds(250));

            return EnterClient(Clients[0]) || Error("Couldn't shell to localhost");
        }

        private IClient FindClient(Socket socket) {
            return _clients.FirstOrDefault(c => c.Socket == socket);
        }
    }
}
using Pyro.Exec;
using System;
using System.Linq;
using System.Net.Sockets;

namespace Pyro.Network.Impl {
    /// <inheritdoc cref="IServer" />
    /// <summary>
    /// A server on the network executes incoming scripts and returns Executor data-stack
    /// to sender.
    /// </summary>
    public class Server
        : NetCommon
        , IServer {
        public override Socket Socket
        {
            get => _listener;
            set => _listener = value;
        }

        public int ListenPort => _port;

        public IDomain Domain => _domain;
        
        public event MessageHandler ReceivedRequest;

        private const int RequestBacklogCount = 50;
        private Socket _listener;
        private readonly int _port;
        private object _receivedRequest;
        private IRegistry _registry => _executionContext.Registry;

        public Server(Peer peer, int port)
            : base(peer) {
            RegisterTypes.Register(_executionContext.Registry);

            _port = port;

            var scope = Exec.Scope;

            _executionContext.Language = Language.ELanguage.Rho;
            scope["peer"] = _Peer;
            scope["server"] = this;
            // work
            //scope["connect"] = TranslateRho("peer.Connect(\"192.168.3.146\", 9999)");

            // home
            scope["connect"] = TranslateRho("peer.Connect(\"192.168.171.1\", 9999)");
            scope["enter"] = TranslateRho("peer.Enter(2)");
            scope["join"] = TranslateRho("assert(connect() && enter())");
            scope["leave"] = TranslateRho("peer.Leave()");
            _executionContext.Language = Language.ELanguage.Pi;
        }

        private Continuation TranslateRho(string text) {
            if (!_executionContext.Translate(text, out var cont)) {
                Error(_executionContext.Error);
                return null;
            }

            return cont;
        }

        public override string ToString() {
            return $"Server: listening on {ListenPort}";
        }

        public bool Start() {
            var endPoint = GetLocalEndPoint(_port);
            if (endPoint == null)
                return Fail($"Couldn't find suitable local endpoint using {_port}");

            var address = endPoint.Address;
            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try {
                _listener.Bind(endPoint);
                _listener.Listen(RequestBacklogCount);

                WriteLine($"Listening on {address}:{_port}");
                Listen();
            } catch (Exception e) {
                Error(e.Message);
                Stop();
                return false;
            }

            return true;
        }

        public void Stop() {
            _Stopping = true;
            _listener?.Close();
            _listener = null;
        }

        protected override bool ProcessReceived(Socket sender, string pi) {
            try {
                if (!RunLocally(pi))
                    return false;

                // clear remote stack, send the datastack back as a list, then expand it to the remote stack, and drop the number of items that 'expand' adds to the stack
                var stack = "clear " + Registry.ToPiScript(Exec.DataStack.ToList()) + " expand drop";
                return Send(sender, stack);
            } catch (Exception e) {
                var msg = $"{e.Message} {e.InnerException?.Message}";
                Exec.Push($"Error: {msg}");
                return Error(msg);
            } finally {
                //FIX ReceivedRequest?.Invoke(this, _Peer.GetClient(sender), pi);
            }
        }

        private bool RunLocally(string pi) {
            if (!_executionContext.Translate(pi, out var continuation)) {
                Exec.Push(_executionContext.Error);
                return Error(_executionContext.Error);
            }

            if (continuation.Code.Count == 0)
                return true;

            continuation = continuation.Code[0] as Continuation;
            if (continuation == null)
                return Error("Server.RunLocally: Continuation expected");

            continuation.Scope = Exec.Scope;
            Exec.Continue(continuation);

            return true;
        }

        private void Listen() {
            _listener.BeginAccept(ConnectRequest, null);
        }

        private void ConnectRequest(IAsyncResult ar) {
            if (_Stopping)
                return;

            var socket = _listener.EndAccept(ar);
            WriteLine($"Serving {socket.RemoteEndPoint}");
            _Peer.NewServerConnection(socket);
            Receive(socket);
            Listen();
        }

        public TIAgent NewAgent<TIAgent>()
            where TIAgent : IAgentBase {
            var agent = Domain.NewAgent<TIAgent>(default);
            return agent;
        }
    }
}


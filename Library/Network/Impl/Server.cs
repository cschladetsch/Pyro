using System;
using System.Linq;
using System.Net.Sockets;

using Pyro.Exec;

namespace Pyro.Network.Impl
{
    /// <inheritdoc cref="IServer" />
    /// <summary>
    /// A server on the network executes incoming scripts and returns Executor data-stack
    /// to sender.
    /// </summary>
    public class Server
        : NetCommon
        , IServer
    {
        public override Socket Socket
        {
            get => _listener;
            set => _listener = value;
        }

        public int ListenPort => _port;
        public event ReceivedResponseHandler ReceivedResponse;

        private const int RequestBacklogCount = 50;
        private Socket _listener;
        private readonly int _port;

        public Server(Peer peer, int port)
            : base(peer)
        {
            RegisterTypes.Register(_Context.Registry);

            _port = port;
            _Exec.Scope["peer"] = _Peer;
            _Exec.Scope["con"] = this;
            _Exec.Scope["connect"] = TranslatePi(@"""192.168.56.1"" 'Connect peer .@ & assert");
            _Exec.Scope["clients"] = TranslatePi("'Clients peer .@");
            _Exec.Scope["test"] = TranslatePi("9999 connect & 1 'RemoteAt peer .@ &");
        }

        protected override bool ProcessReceived(Socket sender, string pi)
        {
            try
            {
                ReceivedResponse?.Invoke(this, _Peer.GetClient(sender), pi);
                RunLocally(pi);
                return SendResponse(sender);
            }
            catch (Exception e)
            {
                return Error($"ProcessReceived: {e.Message}");
            }
        }

        private void RunLocally(string pi)
        {
            if (TranslatePi(pi).Code[0] is Continuation cont)
            {
                cont.Scope = _Exec.Scope;
                _Exec.Continue(cont);
            }
        }

        private bool SendResponse(Socket sender)
        {
            // TODO: Also send _Exec.Scope (?)
            var response = _Registry.ToText(_Exec.DataStack.ToList());
            //WriteLine($"Server sends {response}");
            return Send(sender, response);
        }

        public bool Start()
        {
            var endPoint = GetLocalEndPoint(_port);
            if (endPoint == null)
                return Fail($"Couldn't find suitable local endpoint using {_port}");

            var address = endPoint.Address;
            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listener.Bind(endPoint);
                _listener.Listen(RequestBacklogCount);

                WriteLine($"Listening on {address}:{_port}");
                Listen();
            }
            catch (Exception e)
            {
                Error(e.Message);
                Stop();
                return false;
            }

            return true;
        }

        public void Stop()
        {
            _Stopping = true;
            _listener?.Close();
            _listener = null;
        }

        public bool Execute(string script)
        {
            return _Context.Exec(script);
        }

        private void Listen()
        {
            _listener.BeginAccept(ConnectRequest, null);
        }

        private void ConnectRequest(IAsyncResult ar)
        {
            if (_Stopping)
                return;

            var socket = _listener.EndAccept(ar);
            WriteLine($"Serving {socket.RemoteEndPoint}");
            _Peer.NewConnection(socket);
            Receive(socket);
            Listen();
        }
    }
}

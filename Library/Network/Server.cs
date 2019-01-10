using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Diver.Network
{
    public class Server : NetCommon
    {
        private const int BacklogCount = 50;

        public Server(Peer peer, int port)
            : base(peer)
        {
            _port = port;
            AddTypes(_Context.Registry);
            _Exec.Scope["peer"] = _Peer;
            _Exec.Scope["con"] = this;
            _Exec.Scope["connect"] = TranslatePi(@"""192.168.56.1"" 'Connect peer .@ & assert");
            _Exec.Scope["clients"] = TranslatePi("'Clients peer .@");
            _Exec.Scope["test"] = TranslatePi("9998 connect & clients & 0 at 'Remote peer .@ &");
        }

        private void AddTypes(IRegistry registry)
        {
            Diver.Exec.RegisterTypes.Register(registry);

            //registry.Register(new ClassBuilder<Program>(registry)
            //    .Methods
            //        .Add<string, bool>("Execute", (q, s) => q.Execute(s))
            //    .Class);
            registry.Register(new ClassBuilder<Peer>(registry)
                .Methods
                    .Add<string, int, bool>("Connect", (q, s, p) => q.Connect(s, p))
                    .Add<Socket, bool>("Disconnect", (q, s) => q.Disconnect(s))
                    .Add<Client, bool>("Remote", (q, s) => q.EnterRemote(s))
                    .Add<Client>("Leave", (q, s) => q.Leave())
                .Class);
            registry.Register(new ClassBuilder<Client>(registry)
                .Methods
                    .Add<string, bool>("SendPi", (q, s) => q.SendPi(s))
                .Class);
        }

        protected override void ProcessReceived(Socket sender, string text)
        {
            try
            {
                var cont = TranslatePi(text);
                cont.Scope = _Exec.Scope;
                _Exec.Continue(cont);
                var stack = _Exec.DataStack.ToList();
                var response = _Registry.ToText(stack);
                WriteLine($"Response: {response}");
                _Peer.GetClient(sender)?.SendPi(response);
            }
            catch (Exception e)
            {
                Error($"Exception: {e.Message}");
            }
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
                _listener.Listen(BacklogCount);

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

        private void Listen()
        {
            _listener.BeginAccept(AcceptCallback, _listener);
        }

        public void Stop()
        {
            WriteLine("Server stop");
            _stopping = true;
            _listener?.Close();
            _listener = null;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            var listener = (Socket)ar.AsyncState;
            var socket = listener.EndAccept(ar);
            _Peer.NewConnection(socket);
            Receive(socket);
            Listen();
        }

        private IPEndPoint GetLocalEndPoint(int port)
        {
            var address = GetAddress(Dns.GetHostName());
            if (address == null)
            {
                Error("Couldn't find suitable host address");
                return null;
            }

            return new IPEndPoint(address, port);
        }

        private Socket _listener;
        private readonly int _port;

        public bool Execute(string script)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Diver.Network
{
    public class Server : NetCommon
    {
        public Server(Peer peer, int port)
            : base(peer)
        {
            _port = port;
        }

        public bool Start()
        {
            var endPoint = GetLocalEndPoint(_port);
            if (endPoint == null)
                return Error($"Couldn't find suitable local endpoint using {_port}");

            var address = endPoint.Address;
            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listener.Bind(endPoint);
                _listener.Listen(100);

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
            _peer.NewConnection(socket);
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
    }
}
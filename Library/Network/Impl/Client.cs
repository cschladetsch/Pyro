using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Diver.Exec;
using Con = System.Console;

namespace Diver.Network.Impl
{
    /// <inheritdoc cref="NetCommon" />
    /// <summary>
    /// A connection to a remote server. Can send executable script, and receive
    /// results that are also executable scripts.
    /// </summary>
    public class Client : NetCommon, IClient
    {
        public string HostName => GetHostName();
        public int HostPort => GetHostPort();

        public override Socket Socket
        {
            get => _socket;
            set => _socket = value;
        }

        public Client(Peer peer)
            : base(peer)
        {
        }

        public IEnumerable<string> Results()
        {
            if (_stack == null)
                yield break;

            foreach (var elem in _stack)
                yield return _Context.Registry.ToText(elem);
        }

        public void CompleteConnect(Socket socket)
        {
            _socket = socket;
        }

        public bool Continue(Continuation cont)
        {
            return Send(cont?.ToText());
        }

        public bool Continue(string script)
        {
            return !_Context.Translate(script, out var cont) ? Fail(_Context.Error) : Continue(cont);
        }

        public bool Connect(string hostName, int port)
        {
            var address = GetAddress(hostName);
            if (address == null)
                return Fail($"Couldn't find Ip4 address for {hostName}");
            
            var endPoint = new IPEndPoint(address, port);
            var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(endPoint, Connected, client);

            return true;
        }

        public bool Send(Continuation continuation)
        {
            return Send(continuation.ToText());
        }

        public void Close()
        {
            _Stopping = true;
            _socket.Close();
            _socket = null;
        }

        //public bool ProcessResponse(string response)
        //{
        //    WriteLine($"Recv: {response}");
        //    return true;
        //}

        private void Connected(IAsyncResult ar)
        {
            try
            {
                _socket = (Socket)ar.AsyncState;
                _socket.EndConnect(ar);
                WriteLine($"Client connected to {_socket.RemoteEndPoint}");
                Receive(_socket);
            }
            catch (Exception e)
            {
                Error($"{e.Message}");
            }
        }

        protected override bool ProcessReceived(Socket sender, string pi)
        {
            try
            {
                //WriteLine($"Recv Stack: {pi}");
                if (!_Context.Translate(pi, out var cont))
                    return Error($"Failed to translate {pi}");

                cont.Scope = _Exec.Scope;
                _Exec.Continue(cont);
                _stack = _Exec.Pop<List<object>>();
            }
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }

            return true;
        }

        private int GetHostPort()
        {
            var address = Socket?.RemoteEndPoint as IPEndPoint;
            return address?.Port ?? 0;
        }

        private string GetHostName()
        {
            var address = Socket?.RemoteEndPoint as IPEndPoint;
            return address?.Address.ToString() ?? "none";
        }

        private Socket _socket;
        private IList<object> _stack;

    }
}
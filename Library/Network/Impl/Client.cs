using System;
using System.Collections.Generic;
using System.Net;
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
        public string HostName { get; }
        public int HostPort { get; }

        public Socket Socket
        {
            get => _socket;
            set => _socket = value;
        }

        public Client(Peer peer)
            : base(peer)
        {
        }

        public bool Continue(Continuation cont)
        {
            return Receive(cont?.ToText());
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

        public bool Receive(Continuation continuation)
        {
            return Receive(continuation.ToText());
        }

        public void Close()
        {
            _socket.Close();
            _socket = null;
        }

        public bool Receive(string text)
        {
            var byteData = Encoding.ASCII.GetBytes(text + '~');
            _socket.BeginSend(byteData, 0, byteData.Length, 0, Sent, _socket);
            return true;
        }

        public void WriteDataStackContents(int max = 20)
        {
            Con.ForegroundColor = ConsoleColor.Yellow;
            var str = new StringBuilder();
            if (_stack != null)
            {
                var data = _stack;
                if (data.Count > max)
                    Con.WriteLine("...");
                max = Math.Min(data.Count, max);
                for (var n = max - 1; n >= 0; --n)
                    str.AppendLine($"{n}: {Print(data[n])}");
            }
            Con.Write(str.ToString());
        }

        private void Connected(IAsyncResult ar)
        {
            try
            {
                _socket = (Socket)ar.AsyncState;
                _socket.EndConnect(ar);
                WriteLine($"Client connected via socket {_socket.RemoteEndPoint}");
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
                WriteLine($"Recv Stack: {pi}");
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

        // TODO: split out Client into Client and ConsoleClient : Client

        private string Print(object obj)
        {
            return _Registry.ToText(obj);
        }

        private void Sent(IAsyncResult ar)
        {
            _socket?.EndSend(ar);
        }

        private Socket _socket;
        private IList<object> _stack;
    }
}
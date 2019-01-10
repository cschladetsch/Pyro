using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Diver.Exec;
using Con = System.Console;

namespace Diver.Network
{
    public class Client : NetCommon
    {
        public Socket Socket => _socket;

        public Client(Peer peer)
            : base(peer)
        {
        }

        public void Continue(Continuation cont)
        {
            SendPi(cont?.ToText());
        }

        public bool Connect(string hostName, int port)
        {
            var address = GetAddress(hostName);
            if (address == null)
                return Error($"Couldn't find Ip4 address for {hostName}");
            
            var endPoint = new IPEndPoint(address, port);
            var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(endPoint, ConnectCallback, client);

            return true;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _socket = (Socket)ar.AsyncState;
                _socket.EndConnect(ar);
                //_peer.NewConnection(_socket);
                Receive(_socket);
            }
            catch (Exception e)
            {
                Error($"{e.Message}");
            }
        }

        public bool SendPi(string text)
        {
            var byteData = Encoding.ASCII.GetBytes(text + '~');
            _socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _socket);
            return true;
        }

        protected override void ProcessReceived(Socket sender, string text)
        {
            try
            {
                WriteLine($"Recv Stack: {text}");
                if (!_pi.Translate(text))
                {
                    Error($"Failed to translate {text}");
                    return;
                }
                _exec.Continue(_pi.Result);
                _stack = _exec.Pop<List<object>>();
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }

        // TODO: split out Client into Client and ConsoleClient : Client
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

        private string Print(object obj)
        {
            return _reg.ToText(obj);
        }

        private void SendCallback(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }

        public bool Send(Continuation continuation)
        {
            return SendPi(continuation.ToText());
        }

        private Socket _socket;
        private List<object> _stack;
    }
}
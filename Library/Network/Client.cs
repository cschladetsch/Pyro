using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Diver.Exec;

namespace Diver.Network
{
    public class Client : NetCommon
    {
        public Socket Socket => _socket;

        public Client(Peer peer)
            : base(peer)
        {
        }

        public bool Connect(string hostName, int port)
        {
            var address = Dns.GetHostAddresses(hostName).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            if (address == null)
                return Error($"Couldn't find Ip4 address for {hostName}");
            
            var endPoint = new IPEndPoint(address, port);
            var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(endPoint, ConnectCallback, client);

            return true;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            _socket = (Socket)ar.AsyncState;

            _socket.EndConnect(ar);

            _peer.NewConnection(_socket);

            Receive();
        }

        private void Receive()
        {
            var state = new StateObject {workSocket = _socket};
            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var client = state.workSocket;

            var bytesRead = client.EndReceive(ar);
            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Get the rest of the data.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                return;
            }

            var text = state.sb.ToString();
            WriteLine($"Recv: {text}");

            Receive();
        }

        public bool SendString(string text)
        {
            var byteData = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _socket);
            return true;
        }

        private void SendCallback(IAsyncResult ar)
        {
            int bytesSent = _socket.EndSend(ar);
            WriteLine($"Sent {bytesSent} bytes");
        }

        public bool SendPi(Continuation continuation)
        {
            throw new NotImplementedException();
        }

        private Socket _socket;
    }
}
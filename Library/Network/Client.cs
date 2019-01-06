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
            _socket = (Socket)ar.AsyncState;
            _socket.EndConnect(ar);
            _peer.NewConnection(_socket);

            Receive(_socket);
        }

        public bool SendPi(string text)
        {
            var byteData = Encoding.ASCII.GetBytes(text + '~');
            _socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _socket);
            return true;
        }

        private void SendCallback(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }

        public bool SendPi(Continuation continuation)
        {
            throw new NotImplementedException();
        }

        private Socket _socket;
    }
}
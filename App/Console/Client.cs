using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Console
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
        }

        private Socket _socket;
    }
}
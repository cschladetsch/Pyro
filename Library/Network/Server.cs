using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                // Bind the socket to the local endpoint and listen for incoming connections.  
                _listener.Bind(endPoint);
                _listener.Listen(100);

                WriteLine($"Listening on {address}:{_port}");

                BeginListen();
            }
            catch (Exception e)
            {
                Error(e.Message);
                Stop();
                return false;
            }

            return true;
        }

        private void BeginListen()
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

            var state = new StateObject {workSocket = socket};
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);

            BeginListen();
        }

        private void ReadCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            var state = (StateObject)ar.AsyncState;
            var socket = state.workSocket;

            var bytesRead = socket.EndReceive(ar);
            if (bytesRead <= 0) 
                return;

            // There  might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read more data.  
            var content = state.sb.ToString();
            if (content.IndexOf("eot", StringComparison.Ordinal) > -1)
            {
                var remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;

                WriteLine($"Read {content} from {remoteEndPoint?.Address}");
                _peer.Execute(content);

                Send(socket, @"""Hello!""");
            }

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        private void Send(Socket socket, string data)
        {
            if (_stopping)
                return;

            var bytes = Encoding.ASCII.GetBytes(data);
            socket.BeginSend(bytes, 0, bytes.Length, 0, SendCallback, socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            try
            {
                var socket = (Socket)ar.AsyncState;
                var remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                var bytesSent = socket.EndSend(ar);
                WriteLine($"Sent {bytesSent} to {remoteIpEndPoint?.Address}");
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }

        private static IPEndPoint GetLocalEndPoint(int port)
        {
            var hostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var address = hostInfo.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            if (address == null)
            {
                Error("Couldn't find suitable host address");
                return null;
            }

            return new IPEndPoint(address, port);
        }

        private Socket _listener;
        private readonly int _port;
        private bool _stopping;
    }
}
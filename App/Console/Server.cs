using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Console
{
    public class Server : NetCommon
    {
        public Server(Peer peer)
            : base(peer)
        {
        }

        public void Start(int port)
        {
            var endPoint = GetLocalEndPoint(port);
            if (endPoint == null)
                return;

            var address = endPoint.Address;
            _listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections.  
                _listener.Bind(endPoint);
                _listener.Listen(100);

                WriteLine($"Listening on {address}:{port}");

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    _connectedEvent.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    //WriteLine("Waiting for a connection...");
                    _listener.BeginAccept(AcceptCallback, _listener);

                    // Wait until a connection is made before continuing.  
                    _connectedEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
                Stop();
            }
        }

        public void Stop()
        {
            WriteLine("Server stop");
            _stopping = true;
            _listener?.Close();
            _listener = null;
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

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            _connectedEvent.Set();

            if (_stopping)
                return;

            // Get the socket that handles the client request.  
            var listener = (Socket)ar.AsyncState;
            var handler = listener.EndAccept(ar);

            _peer.NewConnection(listener);

            // Create the state object.  
            var state = new StateObject {workSocket = handler};
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            var state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            var bytesRead = handler.EndReceive(ar);
            if (bytesRead <= 0) 
                return;

            // There  might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read more data.  
            var content = state.sb.ToString();
            if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
            {
                var remoteEndPoint = handler.RemoteEndPoint;

                // All the data has been read from the client. Display it on the console.  
                WriteLine($"Read {content}");

                // Echo the data back to the client.  
                Send(handler, content);
            }
            else
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
        }

        private void Send(Socket handler, string data)
        {
            if (_stopping)
                return;

            // Convert the string data to byte data using ASCII encoding.  
            var bytes = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(bytes, 0, bytes.Length, 0, SendCallback, handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            try
            {
                // Retrieve the socket from the state object.  
                var handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                var bytesSent = handler.EndSend(ar);
                System.Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private Socket _listener;
        private readonly ManualResetEvent _connectedEvent = new ManualResetEvent(false);
        private bool _stopping;
    }
}
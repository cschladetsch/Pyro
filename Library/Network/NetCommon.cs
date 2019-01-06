
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Diver.Exec;

namespace Diver.Network
{
    public class NetCommon : NetworkConsoleWriter
    {
        protected Peer _peer;
        protected Flow.IFactory _factory => _peer.Factory;
        protected Executor _exec => _peer.Executor;

        public NetCommon(Peer peer)
        {
            _peer = peer;
        }

        protected IPAddress GetAddress(string hostname)
        {
            return Dns.GetHostAddresses(hostname).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        protected void Receive(Socket socket)
        {
            var state = new StateObject {workSocket = socket};
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        protected void ReadCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            var state = (StateObject)ar.AsyncState;
            var socket = state.workSocket;

            var bytesRead = socket.EndReceive(ar);
            if (bytesRead <= 0) 
                return;

            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

            var content = state.sb.ToString();
            var end = content.IndexOf('~'); // yes. this means we can't use tilde anywhere.
            if (end >= 0)
            {
                var code = content.Substring(0, end);
                _peer.Execute(code);
                // TODO: Maybe: send _exec.Datastack back?
                state.sb.Clear();
                state.sb.Append(content.Substring(end + 1));
            }

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        protected bool _stopping;
    }
}
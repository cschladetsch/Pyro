using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

using Diver.Exec;
using Pyro.ExecutionContext;

namespace Diver.Network
{
    /// <summary>
    /// Functionality common to both Client and Server aspects of a Peer
    /// </summary>
    public class NetCommon : NetworkConsoleWriter
    {
        protected Peer _Peer;
        protected Context _Context;
        protected Executor _Exec => _Context.Executor;
        protected IRegistry _Registry => _Context.Registry;

        public NetCommon(Peer peer)
        {
            _Peer = peer;
            _Context = new Context();
            Diver.Exec.RegisterTypes.Register(_Context.Registry);
        }

        protected Continuation TranslatePi(string text)
        {
            if (_Context.Translate(text, out var cont)) 
                return cont;

            Error(_Context.Error);
            return null;
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

        protected virtual void ProcessReceived(Socket sender, string text)
        {
            WriteLine($"Recv: {text}");
        }

        protected void ReadCallback(IAsyncResult ar)
        {
            if (_stopping)
                return;

            try
            {
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
                    try
                    {
                        ProcessReceived(socket, code);
                    }
                    catch (Exception e)
                    {
                        Error($"ProcessReceived: {e.Message}");
                    }

                    // reset from end of last continuation
                    state.sb.Clear();
                    state.sb.Append(content.Substring(end + 1));
                }

                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
            catch (Exception e)
            {
                Error($"{e.Message}");
            }
        }

        protected bool _stopping;
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Pyro.Exec;

namespace Pyro.Network.Impl {
    /// <inheritdoc cref="IClient" />
    /// <inheritdoc cref="NetCommon" />
    /// <summary>
    ///     A connection to a remote server. Can send executable script, and receive
    ///     results that are also executable scripts.
    /// </summary>
    public class Client
        : NetCommon
            , IClient {
        private IList<string> _results = new List<string>();

        private Socket _socket;

        public Client(Peer peer)
            : base(peer) {
        }

        public event ClientReceivedHandler OnReceived;

        // TODO: Move to NetCommon
        public string HostName
            => GetHostName();

        public int HostPort
            => GetHostPort();

        public override Socket Socket {
            get => _socket;
            set => _socket = value;
        }

        public bool Continue(string script) {
            return Send(script);
        }

        public void Close() {
            _Stopping = true;
            _socket.Close();
            _socket = null;
        }

        public void GetLatest() {
            // hacks
            Send(" ");
        }

        public bool ContinueRho(string rhoScript) {
            return _executionContext.ExecRho(rhoScript) || Error($"Failed to execute {rhoScript}");
        }

        public override string ToString() {
            return $"Client: connected to {HostName}:{HostPort}";
        }

        public void CompleteConnect(Socket socket) {
            _socket = socket;
        }

        public bool Connect(string hostName, int port) {
            var address = GetAddress(hostName);
            if (address == null) {
                return Fail($"Couldn't find address for {hostName}");
            }

            var endPoint = new IPEndPoint(address, port);
            var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(endPoint, Connected, client);

            return true;
        }

        public bool Send(Continuation continuation) {
            return Send(continuation.ToText());
        }

        private void Connected(IAsyncResult ar) {
            try {
                _socket = (Socket)ar.AsyncState;
                _socket.EndConnect(ar);
                if (!_socket.Connected) {
                    Warn($"Failed to connect to {_socket.RemoteEndPoint}");
                    return;
                }

                WriteLine($"Client: connected to {_socket.RemoteEndPoint} using {_socket.LocalEndPoint}");

                Receive(_socket);
            } catch (Exception e) {
                Error($"{e.Message}");
            }
        }

        protected override bool ProcessReceived(Socket sender, string pi) {
            try {
                if (!_executionContext.Translate(pi, out var cont)) {
                    return Error($"Failed to translate {pi}");
                }

                cont.Scope = Exec.Scope;
                Exec.Continue(cont);

                OnReceived?.Invoke(this, sender);
            } catch (Exception e) {
                Error(e.Message);
                return false;
            }

            return true;
        }

        private int GetHostPort() {
            var address = Socket?.RemoteEndPoint as IPEndPoint;
            return address?.Port ?? 0;
        }

        private string GetHostName() {
            var address = Socket?.RemoteEndPoint as IPEndPoint;
            return address?.Address.ToString() ?? "none";
        }
    }
}
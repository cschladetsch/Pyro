using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;

namespace Diver.Network
{
    /// <summary>
    /// A Peer listens to incoming connections, and can connect to other peers.
    /// </summary>
    public class Peer : NetworkConsoleWriter
    {
        public Flow.IKernel Kernel => _kernel;
        public Flow.IFactory Factory => _kernel.Factory;
        public IRegistry Registry => _registry;
        public List<Socket> Connections => _connections;
        public List<Client> Clients => _clients;
        public Server Server => _server;
        public Client Remote => _remote;
        public string HostName => GetHostName();
        public int HostPort => GetHostPort();

        public Peer(int listenPort)
        {
            _kernel = Flow.Create.Kernel();
            _registry = new Registry();
            _piTranslator = new PiTranslator(_registry);
            _server = new Server(this, listenPort);
        }

        public Client GetClient(Socket sender)
        {
            return _clients.FirstOrDefault(c => c.Socket == sender);
        }

        public string GetHostName()
        {
            return GetRemoteEndPoint()?.Address.ToString();
        }

        private int GetHostPort()
        {
            return GetRemoteEndPoint()?.Port ?? 0;
        }

        public void Start()
        {
            _server.Start();
        }

        public bool EnterRemote(Client client)
        {
            //WriteLine($"Remoting to {client.Socket.RemoteEndPoint}");
            if (!_clients.Contains(client))
                return false;
            _remote = client;
            return true;
        }

        public void Leave()
        {
            //WriteLine($"Going back to local host");
            _remote = null;
        }

        public bool Connect(string hostName, int port)
        {
            var client = new Client(this);
            if (!client.Connect(hostName, port)) 
                return false;

            _clients.Add(client);
            return true;
        }

        public bool Disconnect(Socket socket)
        {
            if (!_connections.Contains(socket))
                return Error($"Not connected to {socket.RemoteEndPoint}");

            socket.Close();
            _connections.Remove(socket);
            return true;
        }

        public void Stop()
        {
            WriteLine($"Closing {_connections.Count} connections");
            foreach (var socket in _connections)
                socket.Close();

            _connections.Clear();
            _clients.Clear();
            _server.Stop();
            _server = null;
        }

        public void Update()
        {
            _kernel.Step();
        }

        public void NewConnection(Socket socket)
        {
            WriteLine($"Connected to {socket.RemoteEndPoint}");
            _connections.Add(socket);
            foreach (var client in _clients)
            {
            }
        }

        public bool Execute(string content)
        {
            try
            {
                if (!_piTranslator.Translate(content))
                {
                    Error($"Failed to translate {content}");
                    return false;
                }

                Continue(_piTranslator.Result);
            }
            catch (Exception e)
            {
                Error($"Exec: {e.Message}");
                return false;
            }

            return true;
        }

        public string GetLocalHostname()
        {
            var address = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return address?.ToString() ?? "localhost";
        }

        public void Continue(Continuation continuation)
        {
            _remote?.Continue(continuation);
        }

        private IPEndPoint GetRemoteEndPoint()
        {
            var socket = Remote?.Socket;
            return socket?.RemoteEndPoint as IPEndPoint;
        }

        private readonly Flow.IKernel _kernel;
        private Server _server;
        private readonly IRegistry _registry;
        private readonly PiTranslator _piTranslator;
        private readonly List<Socket> _connections = new List<Socket>();
        private readonly List<Client> _clients = new List<Client>();
        private Client _remote;
    }
}
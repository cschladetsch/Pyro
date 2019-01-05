using System;
using System.Net.Sockets;
using System.Threading;
using Diver.Exec;

namespace Console
{
    public class Peer : NetworkConsoleWriter
    {
        public Flow.IKernel Kernel => _kernel;
        public Flow.IFactory Factory => _kernel.Factory;
        public Executor Executor => _exec;

        public Peer(Executor exec, int listenPort)
        {
            _exec = exec;
            _kernel = Flow.Create.Kernel();
            _server = new Server(this);
            _serverThread = new Thread(() => _server.Start(listenPort));
            _serverThread.Start();
        }

        public void Update()
        {
            _kernel.Step();
        }

        public void Close()
        {
            if (_serverThread.IsAlive)
                _serverThread.Join(TimeSpan.FromSeconds(2));
            _server.Stop();
        }

        public void NewConnection(Socket listener)
        {
        }

        private readonly Flow.IKernel _kernel;
        private readonly Server _server;
        private readonly Executor _exec;
        private Client _client;
        private readonly Thread _serverThread;
    }
}
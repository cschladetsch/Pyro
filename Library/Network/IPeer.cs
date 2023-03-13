﻿namespace Pyro.Network {
    using Flow;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// General message passed from client to server.
    /// NOTE that some methods are marked by the static analyser as being 'unused'. This is a lie, as they can
    /// be called via script.
    /// </summary>
    public delegate void MessageHandler(IPeer self, IClient client, string text);
    public delegate void ConnectedHandler(IPeer self, IClient client);
    public delegate void OnWriteDelegate(ELogLevel logLevel, string text);

    /// <inheritdoc />
    /// <summary>
    /// A Peer listens to incoming connections, and can connect to other Peers.
    /// </summary>
    public interface IPeer
        : IServer
        {
        event OnWriteDelegate OnWrite;
        
        IDomain Domain { get; }
        
        IServer Local { get; }
        
        IClient Remote { get; }
        
        IList<IClient> Clients { get; }
        
        string LocalHostName { get;  }

        /// <summary>
        /// Fires when we connect to a new client.
        /// </summary>
        event ConnectedHandler OnConnected;

        /// <summary>
        /// Fires when we receive a new request to do some work.
        /// </summary>
        event MessageHandler OnReceivedRequest;

        /// <summary>
        /// Fires when we receive a response to some previous work.
        /// </summary>
        event MessageHandler OnReceivedResponse;

        /// <summary>
        /// Run a local server via loopback.
        /// </summary>
        /// <returns></returns>
        bool SelfHost();

        /// <summary>
        /// Connect to given host and port.
        /// </summary>
        bool Connect(string hostName, int port);

        bool EnterClient(IClient client);
        
        bool Enter(int clientNumber);
        
        bool Execute(string pi);
        
        void Leave();

        void Stop();

        IFuture<Tr> RemoteCall<Tr>(NetId agentId, string methodName);
        
        IFuture<Tr> RemoteCall<Tr, T0>(NetId agentId, string methodName, T0 t0);
        
        IFuture<Tr> RemoteCall<Tr, T0, T1>(NetId agentId, string methodName, T0 t0, T1 t1);
        
        bool SendPiToClient(int n, string piScript);
        
        bool ShowStack(int i);
    }
}


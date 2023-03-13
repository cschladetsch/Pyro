using System;

namespace Pyro.Network {
    /// <inheritdoc />
    /// <summary>
    /// Interface for a general server.
    /// </summary>
    public interface IServer
        : INetCommon {
        int ListenPort { get; }
        
        event MessageHandler ReceivedRequest;

        bool Start();
        
        void Stop();
    }
}
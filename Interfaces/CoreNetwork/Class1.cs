using System;
using System.Collections.Generic;
using Diver.Executor;

namespace Diver.Core.Network
{
    /// <summary>
    /// Common interface to all peers on the network.
    /// </summary>
    public interface IPeer
    {
        string Listen(int port);

        string Connect(string ipAddress, int port);
        string Handshake(Guid node, int port);

        bool SendText(string text);

        void Ping(DateTime sent);
        DateTime Pong();

        List<object> Execute(Diver.IRef<Continuation> cont);
    }

    public interface IResponseBase
    {
    }
}

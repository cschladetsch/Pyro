using System;
using Diver.Exec;

namespace Diver.Core.Network
{
    public enum EResponseType
    {
        None,
        TimedOut,
        ServerError,
        Updating,
    }

    public interface IResponse
    {
        EResponseType Response { get; }
        object Context { get; }
        DateTime Sent { get; }
    }

    /// <summary>
    /// Common interface to all peers on the network.
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// Listen to all incoming connection requests on a well-known port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        bool Listen(int port);

        IResponse HandshakeOffer(Guid node, string ipAddress, int port);
        IResponse HandshakeResponse(Guid node, int port);

        IResponse SendText(string text);

        void Ping(DateTime sent);
        DateTime Pong();

        IResponse Execute(IRef<Continuation> cont);
    }

    public interface IResponseBase
    {
    }
}

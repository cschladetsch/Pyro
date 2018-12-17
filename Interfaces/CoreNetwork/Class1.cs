using System.Collections.Generic;
using Diver.Executor;

namespace Diver.Core.Network
{
    /// <summary>
    /// Common interface to all peers on the network.
    /// </summary>
    public interface IPeer
    {
        string Connect(string ipAddress, int port);
        bool SendText(string text);
        List<object> Execute(IRef<Continuation> cont);
    }
}

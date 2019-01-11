using System.Net.Sockets;
using Diver.Exec;

namespace Diver.Network
{
    public interface IClient : IProcess
    {
        string HostName { get; }
        int HostPort { get; }
        Socket Socket { get; }

        bool Continue(Continuation continuation);
        void Close();
    }
}

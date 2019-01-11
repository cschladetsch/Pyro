using System.Collections.Generic;
using Diver.Exec;

namespace Diver.Network
{
    public interface IClient : INetCommon
    {
        string HostName { get; }
        int HostPort { get; }

        IEnumerable<string> Results();
        bool Continue(Continuation continuation);
        bool Continue(string script);
        void Close();
    }
}

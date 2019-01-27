namespace Diver.Network
{
    using System.Collections.Generic;
    using Diver.Exec;

    /// <summary>
    /// The Client interface.
    /// </summary>
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

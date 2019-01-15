namespace Diver.Network
{
    public interface IServer : INetCommon
    {
        int ListenPort { get; }

        bool Start();
        bool Execute(string pi);
        void Stop();
    }
}
namespace Diver.Network
{
    public interface IServer : IProcess
    {
        bool Start();
        bool Execute(string pi);
        void Stop();
    }
}
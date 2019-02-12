namespace Pyro
{
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}
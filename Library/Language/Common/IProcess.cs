namespace Pyro.Language
{
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}
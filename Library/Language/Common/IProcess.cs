namespace Pyro.Language
{
    // TODO: This is more generic than being in Pyro.Language
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}

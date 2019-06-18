namespace Pyro
{
    /// <summary>
    /// A subtle class that adds rich semantics and usage patterns to derived types.
    /// </summary>
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}


namespace Pyro.Language {
    /// <summary>
    ///     A generalised process.
    ///     TODO: this is more general than living in Pyro.Language namespace.
    /// </summary>
    public interface IProcess {
        bool Failed { get; }
        string Error { get; }
    }
}
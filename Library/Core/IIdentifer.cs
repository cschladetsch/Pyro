namespace Pyro {
    /// <summary>
    ///     DOC
    /// </summary>
    public interface IIdentifer
        : ITextSerialise {
        bool Quoted { get; set; }
    }
}
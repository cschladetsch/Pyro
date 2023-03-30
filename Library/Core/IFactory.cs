namespace Pyro {
    /// <summary>
    ///     Public way to make a new Registry.
    /// </summary>
    public interface IFactory {
        IRegistry NewRegistry();
        ITree NewTree();
    }
}
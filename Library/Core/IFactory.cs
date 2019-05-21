namespace Pyro
{
    /// <summary>
    /// Public way to make a new Regsitry.
    /// </summary>
    public interface IFactory
    {
        IRegistry NewRegistry();
    }
}
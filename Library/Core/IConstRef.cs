namespace Pyro
{
    /// <summary>
    /// A constant-reference to a value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConstRef<out T> 
        : IConstRefBase
    {
        T Value { get; }
    }
}
namespace Pyro
{
    /// <inheritdoc cref="IConstRef{T}" />
    /// <summary>
    /// A mutable reference to an instance of type &lt;T&gt;
    /// </summary>
    /// <typeparam name="T">The instane type.</typeparam>&gt;
    public interface IRef<T>
        : IConstRef<T>
        , IRefBase
    {
        new T Value { get; set; }
    }
}

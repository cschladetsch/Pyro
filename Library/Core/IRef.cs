namespace Pyro
{
    /// <summary>
    /// A reference to another value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRef<T>
        : IConstRef<T>
        , IRefBase
    {
        new T Value { get; set; }
    }
}

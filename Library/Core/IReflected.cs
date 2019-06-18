namespace Pyro
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IReflected
    {
        IRefBase SelfBase { get; set; }
    }

    /// <summary>
    /// A Reflected object of type T
    /// </summary>
    public interface IReflected<T>
    {
        IRef<T> Self { get; }
    }
}


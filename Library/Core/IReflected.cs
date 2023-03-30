namespace Pyro {
    /// <summary>
    ///     Instances of classes that implement this interface have access to
    ///     their <see cref="IRefBase" />
    /// </summary>
    public interface IReflected {
        IRefBase SelfBase { get; set; }
    }

    /// <summary>
    ///     Reflected instances have access to their typed self.
    /// </summary>
    public interface IReflected<T> {
        IRef<T> Self { get; }
    }
}
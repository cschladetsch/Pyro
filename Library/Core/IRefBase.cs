namespace Pyro
{
    /// <inheritdoc />
    /// <summary>
    /// An interface to an object created by a registry.
    /// </summary>
    public interface IRefBase
        : IConstRefBase
    {
        new object BaseValue { get; set; }
    }
}


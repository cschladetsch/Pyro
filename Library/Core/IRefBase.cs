namespace Diver
{
    /// <summary>
    /// An interface to an object created by a registry.
    /// </summary>
    public interface IRefBase : IConstRefBase
    {
        new object BaseValue { get; set; }

        void Set(object value);
    }
}

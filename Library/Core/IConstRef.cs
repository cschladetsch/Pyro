namespace Pyro
{
    public interface IConstRef<out T> : IConstRefBase
    {
        T Value { get; }
    }
}
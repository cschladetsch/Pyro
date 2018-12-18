namespace Diver
{
    public interface IConstRef<out T> : IConstRefBase
    {
        T Value { get; }
    }
}
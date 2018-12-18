namespace Diver
{
    public interface IRef<T> : IRefBase
    {
        T Value { get; set; }
    }
}

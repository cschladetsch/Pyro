namespace Diver.Core
{
    public interface IFuture<T>
    {
        T Value { get; set; }
    }
}
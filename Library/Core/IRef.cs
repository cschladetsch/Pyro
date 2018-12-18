namespace Diver
{
    public interface IRef<T> : IConstRef<T>
    {
        new T Value { get; set; }
    }
}

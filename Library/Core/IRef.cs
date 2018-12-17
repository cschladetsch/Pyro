namespace Diver
{
    public interface IRef<T> : IObjectBase
    {
        T Value { get; set; }
    }
}

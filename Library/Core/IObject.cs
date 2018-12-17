namespace Diver.Core
{
    public interface IObject<T> : IObjectBase
    {
        new T Value { get; set; }
    }
}
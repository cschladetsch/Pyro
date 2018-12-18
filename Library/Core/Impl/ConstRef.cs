namespace Diver.Impl
{
    public class ConstRef<T> : ConstRefBase, IConstRef<T>
    {
        public T Value { get; }
    }
}
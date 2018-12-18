namespace Diver.Impl
{
    public class Ref<T> : RefBase, IRef<T>
    {
        public Ref()
        {
        }

        public Ref(T value)
        {
            Value = value;
        }

        public T Value
        {
            get => (T) BaseValue;
            set => BaseValue = value;
        }
    }
}
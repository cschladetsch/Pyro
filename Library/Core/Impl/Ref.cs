namespace Diver.Impl
{
    public class Ref<T> : ConstRef<T>, IRef<T>
    {
        public Ref()
        {
        }

        public Ref(T value)
        {
            Value = value;
        }

        public new T Value
        {
            get => (T) BaseValue;
            set => BaseValue = value;
        }
    }
}
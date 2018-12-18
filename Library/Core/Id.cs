namespace Diver
{
    public class Id
    {
        public int Value => _value;

        internal Id(int start = 0)
        {
            _value = start;
        }

        public Id(Id prev)
            : this(prev._value + 1)
        {
        }

        public override string ToString()
        {
            return $"#{_value}";
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            var id = obj as Id;
            if (id == null)
                return false;
            return id._value == _value;
        }

        private readonly int _value;
    }
}
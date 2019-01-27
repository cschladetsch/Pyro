namespace Pryo
{
    /// <summary>
    /// Every object in a Registry has a unique Id.
    /// </summary>
    public class Id
    {
        public int Value => _value;

        internal static Id None = new Id(0);

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
            if (!(obj is Id id))
                return false;
            return id._value == _value;
        }

        private readonly int _value;
    }
}
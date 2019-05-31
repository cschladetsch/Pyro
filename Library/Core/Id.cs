namespace Pyro
{
    /// <summary>
    /// Every object in a Registry has a unique Id.
    /// </summary>
    public class Id
    {
        public int Value { get; }

        internal static Id None = new Id(0);

        internal Id(int start = 0)
        {
            Value = start;
        }

        public Id(Id prev)
            : this(prev.Value + 1)
        {
        }

        public override string ToString()
        {
            return $"#{Value}";
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Id id))
                return false;
            return id.Value == Value;
        }
    }
}


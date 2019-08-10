namespace Pyro
{
    /// <summary>
    /// Every object in a Registry has a unique Id.
    /// </summary>
    public class Id
    {
        public int Value { get; }

        internal static Id None = new Id(0);
        internal Id(int start = 0) => Value = start;

        public Id(Id prev)
            : this(prev.Value + 1)
        {
        }

        public override string ToString()
            => $"#{Value}";

        public override int GetHashCode()
            => Value;

        public override bool Equals(object obj) =>
            obj is Id id && id.Value == Value;
    }
}


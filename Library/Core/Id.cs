namespace Pyro {
    /// <summary>
    ///     Every object in a Registry has a unique Id.
    /// </summary>
    public class Id {
        internal static Id None = new Id();

        internal Id(int num = 0) {
            Value = num;
        }

        public Id(Id prev)
            : this(prev.Value + 1) {
        }

        public int Value { get; }

        public override string ToString() {
            return $"#{Value}";
        }

        public override int GetHashCode() {
            return Value;
        }

        public override bool Equals(object obj) {
            return obj is Id id && id.Value == Value;
        }
    }
}
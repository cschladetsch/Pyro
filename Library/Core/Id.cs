namespace Pyro {
    /// <summary>
    ///     Every object in a Registry has a unique Id.
    /// </summary>
    public class Id {
        public Id(int num = 0) {
            Value = num;
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
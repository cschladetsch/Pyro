namespace Pyro {
    /// <inheritdoc />
    /// <summary>
    /// A singular label (part of a path)
    /// </summary>
    public class Label : IdentBase {
        /// <summary>
        /// TODO: make this is a property
        /// </summary>
        public readonly string Text;

        public Label(string text, bool quoted = false)
            : base(quoted) {
            Text = text;
        }

        public override string ToString()
            => $"{(Quoted ? "\'" : "")}{Text}";
    }
}


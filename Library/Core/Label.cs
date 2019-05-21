namespace Pyro
{
    /// <summary>
    /// A singular label (part of a path)
    /// </summary>
    public class Label
        : IdentBase
    {
        /// <summary>
        /// TODO: make this is a property
        /// </summary>
        public string Text;

        public Label(string text, bool quoted = false)
            : base(quoted)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $"{(Quoted ? "\'" : "")}{Text}";
        }
    }
}

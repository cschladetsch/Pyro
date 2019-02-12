namespace Pyro
{
    public class Label : IdentBase
    {
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

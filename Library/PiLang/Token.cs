namespace Diver.PiLang
{
    public class Token
    {
        public ETokenType Type;
        public Slice Slice;
        public int LineNumber;
        public Lexer Lexer;
        public string Text => Lexer.GetText(LineNumber, Slice);

        public override string ToString()
        {
            return $"{Type}{Text}";
        }
    }
}
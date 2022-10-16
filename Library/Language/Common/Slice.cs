namespace Pyro.Language {
    /// <summary>
    /// A text segment in a Lexer.
    /// </summary>
    public struct Slice {
        public int Length => End - Start;
        public int LineNumber;

        public readonly int Start;
        public readonly int End;
        public readonly LexerBase Lexer;
        public string Text => ToString();

        public Slice(LexerBase lexer, int ln, int start, int end)
            : this(lexer, start, end) {
            LineNumber = ln;
        }

        public Slice(LexerBase lexer, int start, int end) {
            Start = start;
            End = end;
            Lexer = lexer;
            LineNumber = lexer.LineNumber;
        }

        public override string ToString() {
            return Lexer.GetText(this);
        }
    }
}

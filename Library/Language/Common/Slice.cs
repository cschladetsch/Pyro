using System.Net.Mime;

namespace Diver.Language
{
    /// <summary>
    /// A segment in a lexer
    /// </summary>
    public struct Slice
    {
        public int Start, End;
        public int Length => End - Start;
        public int LineNumber;
        public LexerBase Lexer;
        public string Text => ToString();

        public Slice(LexerBase lexer, int ln, int start, int end)
            : this(lexer, start, end)
        {
            LineNumber = ln;
        }

        public Slice(LexerBase lexer, int start, int end)
        {
            Start = start;
            End = end;
            Lexer = lexer;
            LineNumber = lexer.LineNumber;
        }

        public override string ToString()
        {
            return Lexer.GetText(this);
        }
    }
}
namespace Pyro.Language
{
    using System;
    using System.Collections.Generic;

    /// <inheritdoc cref="Process" />
    /// <inheritdoc cref="ILexer" />
    /// <summary>
    /// Common to all lexers. Provides basic lexing functionality that is not specific to
    /// any token-type.
    /// </summary>
    public class LexerBase
        : Process
        , ILexer
    {
        public string Input => _input;
        public int Offset => _offset;
        public int LineNumber => _lineNumber;
        public string Line => Lines[_lineNumber];
        public List<string> Lines { get; } = new List<string>();

        protected string _input;
        protected int _offset, _lineNumber;
        protected virtual void AddStringToken(Slice slice) { }
        protected virtual void LexError(string fmt, params object[] args) { }

        public LexerBase(string input)
            => _input = UnfuckNewLines(input);

        public string GetLine(int lineNumber)
            => Lines[lineNumber];

        public string GetText(Slice slice)
            => Lines[slice.LineNumber].Substring(slice.Start, slice.Length);

        protected void CreateLines()
        {
            if (string.IsNullOrEmpty(_input))
                return;

            const char newLine = '\n';
            if (_input[_input.Length - 1] != newLine)
                _input += newLine;

            int lineStart = 0, n = 0;
            foreach (var c in _input)
            {
                if (c == newLine)
                {
                    Lines.Add(_input.Substring(lineStart, n - lineStart + 1));
                    lineStart = n + 1;
                }

                ++n;
            }
        }

        protected bool LexString()
        {
            var start = _offset;
            Next();
            while (!Failed && Current() != '"')
            {
                if (Current() == '\\')
                {
                    switch (Next())
                    {
                    case '"':
                    case 'n':
                    case 't':
                        break;

                    default:
                        LexError("Bad escape sequence %c");
                        return false;
                    }
                }

                if (Peek() == 0)
                {
                    Fail("Bad string literal");
                    return false;
                }

                Next();
            }

            Next();

            // the +1 and -1 to remove the start and end double quote " characters
            AddStringToken(new Slice(this, start + 1, _offset - 1));

            return true;
        }

        protected char Current()
        {
            if (_lineNumber == Lines.Count)
                return (char)0;

            return Line[_offset];
        }

        protected char Next()
        {
            if (EndOfLine())
            {
                _offset = 0;
                ++_lineNumber;
            }
            else
                ++_offset;

            if (_lineNumber == Lines.Count)
                return (char)0;

            return Line[_offset];
        }

        protected bool EndOfLine()
        {
            var len = Line.Length;
            return len == 0 || Offset == len - 1;
        }

        /// <summary>
        /// Peek N glyphs ahead on current line
        /// </summary>
        /// <param name="n">The number of glyphs to look ahead</param>
        /// <returns>(char)0 if cannot peek, else the char peeked</returns>
        protected char Peek(int n = 1)
        {
            const char end = (char)0;

            if (Current() == 0)
                return end;

            if (EndOfLine())
                return end;

            return _offset + n >= Line.Length ? end : Line[_offset + n];
        }

        protected Slice Gather(Func<char, bool> filter)
        {
            var start = _offset;
            while (filter(Next()))
                /* skip */;

            return new Slice(this, start, _offset);
        }

        protected bool IdentOrKeyword()
        {
            AddKeywordOrIdent(GatherIdent());
            return true;
        }

        protected virtual void AddKeywordOrIdent(Slice gatherIdent)
            => throw new NotImplementedException();

        protected bool IsValidIdentGlyph(char ch)
            => char.IsLetterOrDigit(ch) || ch == '_';

        protected Slice GatherIdent()
        {
            var begin = _offset;
            Next();
            while (IsValidIdentGlyph(Current()))
                Next();
            return new Slice(this, begin, _offset);
        }

        protected bool IsSpaceChar(char arg)
            => char.IsWhiteSpace(arg);

        private string UnfuckNewLines(string input)
            => input.Replace("\r", "");
    }
}


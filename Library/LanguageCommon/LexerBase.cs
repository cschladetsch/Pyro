using System;
using System.Collections.Generic;

namespace Diver.LanguageCommon
{
    /// <summary>
    /// Common to all lexers. Provides basic lexing functionality that is not specific to
    /// any token-type.
    /// </summary>
    public class LexerBase : Process
    {
        public string Input => _input;
        public int Offset => _offset;
        public int LineNumber => _lineNumber;
        public string Line => _lines[_lineNumber];
        public List<string> Lines => _lines;

        protected virtual void AddStringToken(int ln, Slice slice) { }
        protected virtual void LexError(string fmt, params object[] args) { }

        public LexerBase(string input)
        {
            _input = input;
        }

        public string GetLine(int lineNumber)
        {
            return _lines[lineNumber];
        }

        public string GetText(int lineNumber, Slice slice)
        {
            return _lines[lineNumber].Substring(slice.Start, slice.End);
        }

        protected void CreateLines()
        {
            var newLine = '\n';
            if (_input[_input.Length - 1] != newLine)
                _input += newLine;

            int lineStart = 0, n = 0;
            foreach (char c in _input)
            {
                if (c == newLine)
                {
                    _lines.Add(_input.Substring(lineStart, n - lineStart + 1));
                    lineStart = n + 1;
                }

                ++n;
            }
        }

        protected bool LexString()
        {
            int start = _offset;
            Next();
            while (!Failed && Current() != '"') // "
            {
                if (Current() == '\\')
                {
                    switch (Next())
                    {
                    case '"':    // "
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
            AddStringToken(_lineNumber, new Slice(start + 1, _offset - 1));

            return true;
        }

        protected char Current()
        {
            if (_lineNumber == _lines.Count)
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

            if (_lineNumber == _lines.Count)
                return (char)0;

            return Line[_offset];
        }

        protected bool EndOfLine()
        {
            var len = Line.Length;
            return len == 0 || Offset == len - 1;
        }

        protected char Peek()
        {
            if (Current() == 0)
                return (char) 0;
            if (EndOfLine())
                return (char) 0;

            return Line[_offset + 1];
        }

        protected Slice Gather(Func<char, bool> filter)
        {
            var start = _offset;
            while (filter(Next()))
                ;

            return new Slice(start, _offset);
        }

        private readonly  List<string> _lines = new List<string>();
        protected string _input;
        protected int _offset, _lineNumber;
    }
}

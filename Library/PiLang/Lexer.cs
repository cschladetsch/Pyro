using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Diver.PiLang
{
    public class Lexer
    {
        public string Input => _input;
        public int Offset => _offset;
        public int LineNumber => _lineNumber;
        public string Line => _lines[_lineNumber];

        public Lexer(string input)
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

        private void CreateLines()
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

        bool LexString()
        {
            return false;
        }

        private char Current()
        {
            if (_lineNumber == _lines.Count)
                return (char)0;
            return Line[_offset];
        }

        private char Next()
        {
            return ' ';
        }

        private bool EndOfLine()
        {
            var len = Line.Length;
            return len == 0 || Offset == len - 1;
        }

        private char Peek()
        {
            if (Current() == 0)
                return (char) 0;
            if (EndOfLine())
                return (char) 0;

            return Line[_offset + 1];
        }

        private Slice Gather(Func<char, bool> filter)
        {
            var start = _offset;
            while (filter(Next()))
                ;

            return new Slice(start, _offset);
        }

        private readonly  List<string> _lines = new List<string>();
        private string _input;
        private int _offset, _lineNumber;
    }
}

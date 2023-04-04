using System;
using System.Collections.Generic;

namespace Pyro.Language {
    /// <inheritdoc cref="Process" />
    /// <inheritdoc cref="ILexer" />
    /// <summary>
    ///     Common to all lexers. Provides basic lexing functionality that is not specific to
    ///     any token-type.
    /// </summary>
    public class LexerBase
        : Process
            , ILexer {

        public int LineNumber => _lineNumber;

        public List<string> Lines { get; } = new List<string>();

        public string Input { get; private set; }

        protected int _offset, _lineNumber;

        private int Offset => _offset;
    
        private string Line => Lines[_lineNumber];


        protected virtual void AddStringToken(Slice slice) {
        }

        protected virtual void LexError(string fmt, params object[] args) {
        }

        public string GetLine(int lineNumber) {
            return Lines[lineNumber];
        }

        public string GetText(Slice slice) {
            return Lines[slice.LineNumber].Substring(slice.Start, slice.Length);
        }

        protected void CreateLines() {
            if (string.IsNullOrEmpty(Input)) {
                return;
            }

            const char newLine = '\n';
            if (Input[Input.Length - 1] != newLine) {
                Input += newLine;
            }

            int lineStart = 0, n = 0;
            foreach (var c in Input) {
                if (c == newLine) {
                    Lines.Add(Input.Substring(lineStart, n - lineStart + 1));
                    lineStart = n + 1;
                }

                ++n;
            }
        }

        protected bool LexString() {
            var start = _offset;
            Next();
            while (!Failed && Current() != '"') {
                if (Current() == '\\') {
                    switch (Next()) {
                        case '"':
                        case 'n':
                        case 't':
                            break;

                        default:
                            LexError("Bad escape sequence %c");
                            return false;
                    }
                }

                if (Peek() == 0) {
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

        protected char Current() {
            if (_lineNumber == Lines.Count) {
                return (char)0;
            }

            return Line[_offset];
        }

        protected char Next() {
            if (EndOfLine()) {
                _offset = 0;
                ++_lineNumber;
            }
            else {
                ++_offset;
            }

            if (_lineNumber == Lines.Count) {
                return (char)0;
            }

            return Line[_offset];
        }

        private bool EndOfLine() {
            var len = Line.Length;
            return len == 0 || Offset == len - 1;
        }

        protected char Peek(int n = 1) {
            const char end = (char)0;

            if (Current() == 0) {
                return end;
            }

            if (EndOfLine()) {
                return end;
            }

            return _offset + n >= Line.Length ? end : Line[_offset + n];
        }

        protected Slice Gather(Func<char, bool> filter) {
            var start = _offset;
            while (!EndOfLine() && filter(Next()))
                /* skip */
                ;

            return new Slice(this, start, _offset);
        }

        protected bool IdentOrKeyword() {
            AddKeywordOrIdent(GatherIdent());
            return true;
        }

        protected virtual void AddKeywordOrIdent(Slice gatherIdent) {
            throw new NotImplementedException();
        }

        private static bool IsValidIdentGlyph(char ch) {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }

        private Slice GatherIdent() {
            var begin = _offset;
            Next();
            while (IsValidIdentGlyph(Current()))
                Next();
            return new Slice(this, begin, _offset);
        }

        protected static bool IsSpaceChar(char arg) {
            return char.IsWhiteSpace(arg);
        }

        private string UnfuckNewLines(string input) {
            return input.Replace("\r", "");
        }
    }
}

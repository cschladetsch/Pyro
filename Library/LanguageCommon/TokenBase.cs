using System.Runtime.InteropServices;
using System.Text;

namespace Diver
{
    public class TokenBase<ETokenType> : ITokenBase<ETokenType>
    {
        public Slice Slice => _slice;
        public ETokenType Type { get; set; }
        public int LineNumber => _lineNumber;
        public LexerBase Lexer => _lexer;
        public string Text => Lexer.GetText(LineNumber, Slice);

        public TokenBase()
        {
        }

        public TokenBase(ETokenType type, LexerBase lexer, int ln, Slice slice)
        {
            _type = type;
            _lineNumber = ln;
            _slice = slice;
            _lexer = lexer;
        }

        public char this[int n] => _lexer.Input[_slice.Start + n];

        public override string ToString()
        {
            var str = new StringBuilder();
            var ty = _type.ToString();
            var text = GetText();
            str.Append(ty);
            if (ty != text && !string.IsNullOrEmpty(text))
            {
                str.Append($" '{text}'");
            }

            return str.ToString();
        }

        public string GetText()
        {
            if (_lexer == null)
                return "";

            if (_slice.Length == 0)
                return "";

            return _lexer.GetLine(_lineNumber).Substring(_slice.Start, _slice.Length);
        }

        public ETokenType _type;
        private Slice _slice;
        private readonly LexerBase _lexer;
        private readonly int _lineNumber;
    }
}

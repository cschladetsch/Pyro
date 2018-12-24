using System.Text;

namespace Diver.Language
{
    public class TokenBase<ETokenType> : ITokenBase<ETokenType>
    {
        public Slice Slice => _slice;
        public ETokenType Type { get => _type; set => _type = value; } 
        public int LineNumber => _slice.LineNumber;
        public LexerBase Lexer => _slice.Lexer;
        public string Text => Lexer.GetText(Slice);
        public char this[int n] => Lexer.Input[_slice.Start + n];

        public TokenBase()
        {
        }

        public TokenBase(ETokenType type, Slice slice)
        {
            _type = type;
            _slice = slice;
        }

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
            if (Lexer == null)
                return "";

            if (_slice.Length == 0)
                return "";

            return Lexer.GetLine(LineNumber).Substring(_slice.Start, _slice.Length);
        }

        public ETokenType _type;
        private readonly Slice _slice;
    }
}

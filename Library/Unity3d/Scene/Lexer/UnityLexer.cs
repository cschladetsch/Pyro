using Diver.Language;
using Diver.Language.Impl;

namespace Pyro.Unity3d.Scene
{
    public class UnityLexer
        : LexerCommon<EToken, Token, TokenFactory>
    {
        public UnityLexer(string input)
            : base(input)
        {
        }

        protected override void AddKeyWords()
        {
        }

        protected override bool NextToken()
        {
            var current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            //if (char.IsDigit(current))
            //    return AddSlice(EToken.Int, Gather(char.IsDigit));

            switch (current)
            {
                case '%': return Add(EToken.Percent);
                case '&': return Add(EToken.Ampersand);
                case ' ': return AddIfNext(' ', EToken.Indent, EToken.Space);
                case '\r': // fuck MS-DOS
                case '\n': return Add(EToken.NewLine);
                case '!': return Add(EToken.Bang);
                case ':': return Add(EToken.Colon);
                case '{': return Add(EToken.OpenBrace);
                case '}': return Add(EToken.CloseBrace);
                case '[': return Add(EToken.OpenSquareBracket);
                case ']': return Add(EToken.CloseSquareBracket);
                case '-':
                {
                    if (char.IsDigit(Peek()))
                        return AddNumber();
                    if (Peek() != '-')
                        return Add(EToken.Dash);
                    return Peek(2) == '-' 
                        ? AddThreeCharOp(EToken.Document) 
                        : LexError("-- not expected");
                }
            }

            LexError($"Unrecognised '{current}'");

            return false;
        }

        private bool AddNumber()
        {
            return AddSlice(EToken.Int, Gather(char.IsDigit));
        }
    }
}
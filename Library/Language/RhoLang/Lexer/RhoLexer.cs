using Diver.RhoLang;

namespace Diver.Language
{
    /// <summary>
    /// Lexer for the Rho language
    /// </summary>
    public class RhoLexer 
        : LexerCommon<ERhoToken, RhoToken, RhoTokenFactory>
    {
        public RhoLexer(string input)
            : base(input)
        {
        }

        protected override void AddKeyWords()
        {
            _keyWords.Add("break", ERhoToken.Break);
            _keyWords.Add("if", ERhoToken.If);
            _keyWords.Add("else", ERhoToken.Else);
            _keyWords.Add("for", ERhoToken.For);
            _keyWords.Add("true", ERhoToken.True);
            _keyWords.Add("false", ERhoToken.False);
            _keyWords.Add("yield", ERhoToken.Yield);
            _keyWords.Add("fun", ERhoToken.Fun);
            _keyWords.Add("return", ERhoToken.Return);
            _keyWords.Add("self", ERhoToken.Self);
            _keyWords.Add("while", ERhoToken.While);
            _keyWords.Add("assert", ERhoToken.Assert);
        }

        protected override bool NextToken()
        { 
            char current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            if (char.IsDigit(current))
                return AddSlice(ERhoToken.Int, Gather(char.IsDigit));

            switch (current)
            {
            case Pathname.Quote: return Add(ERhoToken.Quote);
            case '{': return Add(ERhoToken.OpenBrace);
            case '}': return Add(ERhoToken.CloseBrace);
            case '(': return Add(ERhoToken.OpenParan);
            case ')': return Add(ERhoToken.CloseParan);
            case ':': return Add(ERhoToken.Colon);
            case ' ': return AddSlice(ERhoToken.Whitespace, Gather(IsSpaceChar));
            case ',': return Add(ERhoToken.Comma);
            case '*': return Add(ERhoToken.Multiply);
            case '[': return Add(ERhoToken.OpenSquareBracket);
            case ']': return Add(ERhoToken.CloseSquareBracket);
            case '=': return AddIfNext('=', ERhoToken.Equiv, ERhoToken.Assign);
            case '!': return AddIfNext('=', ERhoToken.NotEquiv, ERhoToken.Not);
            case '&': return AddIfNext('&', ERhoToken.And, ERhoToken.BitAnd);
            case '|': return AddIfNext('|', ERhoToken.Or, ERhoToken.BitOr);
            case '<': return AddIfNext('=', ERhoToken.LessEquiv, ERhoToken.Less);
            case '>': return AddIfNext('=', ERhoToken.GreaterEquiv, ERhoToken.Greater);
            case '"': return LexString(); 
            case '^': return Add(ERhoToken.Xor);
            case '\t': return Add(ERhoToken.Tab);
            case '\r': 
            case '\n': return Add(ERhoToken.NewLine);
            case '-':
                if (char.IsDigit(Peek()))
                    return AddSlice(ERhoToken.Int, Gather(char.IsDigit));
                if (Peek() == '-')
                    return AddTwoCharOp(ERhoToken.Decrement);
                if (Peek() == '=')
                    return AddTwoCharOp(ERhoToken.MinusAssign);
                return Add(ERhoToken.Minus);

            case '.':
                if (Peek() == '.')
                {
                    Next();
                    if (Peek() == '.')
                    {
                        Next();
                        return Add(ERhoToken.Resume, 3);
                    }
                    return Fail("Two dots doesn't work");
                }
                return Add(ERhoToken.Dot);

            case '+':
                if (Peek() == '+')
                    return AddTwoCharOp(ERhoToken.Increment);
                if (Peek() == '=')
                    return AddTwoCharOp(ERhoToken.PlusAssign);
                return Add(ERhoToken.Plus);

            case '/':
                if (Peek() == '/')
                {
                    Next();
                    var start = _offset + 1;
                    while (Next() != '\n')
                        ;

                    var comment = _factory.NewToken(
                        ERhoToken.Comment,
                        new Slice(this, start, _offset));
                    _tokens.Add(comment);
                    return true;
                }

                //return LexError("/ is not a valid RhoToken");//Add(ERhoToken.Divide);
                return Add(ERhoToken.Separator);
            }

            LexError($"Unrecognised '{current}'");

            return false;
        }

        protected override void AddKeywordOrIdent(Slice slice)
        {
            if (_keyWords.TryGetValue(slice.Text, out var tok))
                _tokens.Add(_factory.NewToken(tok, slice));
            else
                _tokens.Add(_factory.NewTokenIdent(slice));
        }

        protected override void Terminate()
        {
        }
    }
}

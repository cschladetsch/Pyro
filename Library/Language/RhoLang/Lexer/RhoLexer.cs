namespace Pyro.RhoLang.Lexer
{
    using Language;
    using Pyro.Language.Impl;

    /// <inheritdoc />
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
            _KeyWords.Add("break", ERhoToken.Break);
            _KeyWords.Add("if", ERhoToken.If);
            _KeyWords.Add("else", ERhoToken.Else);
            _KeyWords.Add("for", ERhoToken.For);
            _KeyWords.Add("true", ERhoToken.True);
            _KeyWords.Add("false", ERhoToken.False);
            _KeyWords.Add("yield", ERhoToken.Yield);
            _KeyWords.Add("fun", ERhoToken.Fun);
            _KeyWords.Add("return", ERhoToken.Return);
            _KeyWords.Add("self", ERhoToken.Self);
            _KeyWords.Add("while", ERhoToken.While);
            _KeyWords.Add("assert", ERhoToken.Assert);
            _KeyWords.Add("writeln", ERhoToken.WriteLine);
            _KeyWords.Add("write", ERhoToken.Write);
            _KeyWords.Add("new", ERhoToken.New);
            _KeyWords.Add("in", ERhoToken.In);
            _KeyWords.Add("class", ERhoToken.Class);
        }

        protected override bool NextToken()
        {
            var current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            // TODO: this handled differently (and poorly) in PiLexer.
            if (char.IsDigit(current))
            {
                var start = Gather(char.IsDigit);

                // TODO: floating point numbers
                //if (Current() == '.')
                //{
                //    Next();
                //    var frac = Gather(char.IsDigit);
                //    AddSlice(ERhoToken.Float, new Slice(this, start.Start, frac.End));
                //}
                //else
                //    AddSlice(ERhoToken.Int, start);

                return AddSlice(ERhoToken.Int, start);
            }

            switch (current)
            {
            case Pathname.Quote: return Add(ERhoToken.Quote);
            // TODO: This means we can't use ` at all in embedded Pi code.
            case '`': return AddEmbeddedPi();
            case '{': return Add(ERhoToken.OpenBrace);
            case '}': return Add(ERhoToken.CloseBrace);
            case '(': return Add(ERhoToken.OpenParan);
            case ')': return Add(ERhoToken.CloseParan);
            case ':': return Add(ERhoToken.Colon);
            case ' ': return AddSlice(ERhoToken.Space, Gather(c => c == ' '));
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
            {
                // fuck I hate this
                Next();
                return true;
            }
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
                    return Fail("Two dots doesn't work.");
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
                        /* skip comment */;

                    var comment = _Factory.NewToken(
                        ERhoToken.Comment,
                        new Slice(this, start, _offset));
                    _Tokens.Add(comment);
                    return true;
                }

                //return LexError("/ is not a valid RhoToken");//Add(ERhoToken.Divide);
                return Add(ERhoToken.Separator);
            }

            LexError($"Unrecognised '{current}'.");

            return false;
        }

        private bool AddEmbeddedPi()
        {
            Next();
            AddSlice(ERhoToken.PiSlice, Gather(c => c != '`'));
            Next();
            return true;
        }

        protected override void AddKeywordOrIdent(Slice slice)
        {
            _Tokens.Add(_KeyWords.TryGetValue(slice.Text, out var tok)
                ? _Factory.NewToken(tok, slice)
                : _Factory.NewTokenIdent(slice));
        }

        protected override void Terminate()
            => _Tokens.Add(_Factory.NewToken(ERhoToken.Nop, new Slice(this, _offset, _offset)));
    }
}


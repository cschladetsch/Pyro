namespace Pyro.Language.Lexer
{
    using System.Collections.Generic;
    using Exec;
    using Impl;

    /// <inheritdoc />
    /// <summary>
    /// A Pi-lang lexer. Converts a string to a sequence of Pi tokens.
    /// </summary>
    public partial class PiLexer
        : LexerCommon<EPiToken, PiToken, PiTokenFactory>
    {
        public static Dictionary<EOperation, EPiToken> _opToToken = new Dictionary<EOperation, EPiToken>();

        public PiLexer(string input)
            : base(input)
            => CreateOpToToken();

        protected override bool NextToken()
        {
            var current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            if (char.IsDigit(current))
                return AddSlice(EPiToken.Int, Gather(char.IsDigit));

            switch (current)
            {
            case Pathname.Quote: return Add(EPiToken.Quote);
            case '`': return AddQuotedOperation();
            case '{': return Add(EPiToken.OpenBrace);
            case '}': return Add(EPiToken.CloseBrace);
            case '(': return Add(EPiToken.OpenParan);
            case ')': return Add(EPiToken.CloseParan);
            case ':': return Add(EPiToken.Colon);
            case ' ': return AddSlice(EPiToken.Whitespace, Gather(IsSpaceChar));
            case '@': return Add(EPiToken.Retrieve);
            case ',': return Add(EPiToken.Comma);
            case '#': return Add(EPiToken.Store);
            case '*': return Add(EPiToken.Multiply);
            case '[': return Add(EPiToken.OpenSquareBracket);
            case ']': return Add(EPiToken.CloseSquareBracket);
            case '=': return AddIfNext('=', EPiToken.Equiv, EPiToken.Assign);
            case '!': return AddIfNext('=', EPiToken.NotEquiv, EPiToken.Replace);
            case '&': return AddIfNext('&', EPiToken.And, EPiToken.Suspend);
            case '|': return AddIfNext('|', EPiToken.Or, EPiToken.BitOr);
            case '<': return AddIfNext('=', EPiToken.LessEquiv, EPiToken.LessEquiv);
            case '>': return AddIfNext('=', EPiToken.GreaterEquiv, EPiToken.Greater);
            case '"': return LexString();
            case '^': return Add(EPiToken.Xor);
            case '\t': return Add(EPiToken.Tab);
            case '\r':
            case '\n': return Add(EPiToken.NewLine);
            case '-':
                if (char.IsDigit(Peek()))
                    return AddSlice(EPiToken.Int, Gather(char.IsDigit));
                if (Peek() == '-')
                    return AddTwoCharOp(EPiToken.Decrement);
                if (Peek() == '=')
                    return AddTwoCharOp(EPiToken.MinusAssign);
                return Add(EPiToken.Minus);

            case '.':
                if (Peek() == '.')
                {
                    Next();
                    if (Peek() != '.')
                        return Fail("Two dots doesn't work");

                    Next();
                    return Add(EPiToken.Resume, 3);
                }
                else if (Peek() == '@')
                    return AddTwoCharOp(EPiToken.GetMember);

                // test for floating-point numbers
                var prev = PrevToken();
                if (prev.Type == EPiToken.Int)
                {
                    // remove the previously added int
                    // TODO: should really do this before adding the first int...
                    _Tokens.RemoveAt(_Tokens.Count - 1);
                    return AddSlice(
                        EPiToken.Float,
                        new Slice(this, prev.LineNumber, prev.Slice.Start, _offset));
                }

                return Add(EPiToken.Dot);

            case '+':
                if (Peek() == '+')
                    return AddTwoCharOp(EPiToken.Increment);
                if (Peek() == '=')
                    return AddTwoCharOp(EPiToken.PlusAssign);
                return Add(EPiToken.Plus);

            case '/':
                if (Peek() != '/')
                    return Add(EPiToken.Divide);

                Next();
                var start = _offset + 1;
                while (Next() != '\n')
                    /* skip to eol*/;

                _Tokens.Add(_Factory.NewToken(
                    EPiToken.Comment,
                    new Slice(this, start, _offset)));

                return true;
            }

            return LexError($"Unrecognised PiToken '{current}'");
        }

        private PiToken PrevToken()
        {
            var count = _Tokens.Count;
            return count >= 1 ? _Tokens[count - 1] : new PiToken(EPiToken.None, new Slice());
        }

        private bool AddQuotedOperation()
        {
            Next();
            if (!int.TryParse(Gather(char.IsDigit).Text, out var num))
                return Fail("Operation number expected");
            var op = (EOperation) num;
            return _opToToken.TryGetValue(op, out var tok) && Add(tok);
        }

        protected override bool Fail(string err)
            => base.Fail($"{_lineNumber}({_offset}): {err}");

        protected override void AddKeywordOrIdent(Slice slice)
            => _Tokens.Add(_KeyWords.TryGetValue(slice.Text, out var tok)
            ? _Factory.NewToken(tok, slice)
            : _Factory.NewTokenIdent(slice));

        protected override void Terminate()
        {
        }
    }
}


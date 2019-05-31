using System.Collections.Generic;

namespace Pyro.Language.Lexer
{
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
            : base(input) => CreateOpToToken();

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
                    if (Peek() == '.')
                    {
                        Next();
                        return Add(EPiToken.Resume, 3);
                    }
                    return Fail("Two dots doesn't work");
                }
                else if (Peek() == '@')
                {
                    return AddTwoCharOp(EPiToken.GetMember);
                }

                return Add(EPiToken.Dot);

            case '+':
                if (Peek() == '+')
                    return AddTwoCharOp(EPiToken.Increment);
                if (Peek() == '=')
                    return AddTwoCharOp(EPiToken.PlusAssign);
                return Add(EPiToken.Plus);

            case '/':
                if (Peek() == '/')
                {
                    Next();
                    var start = _offset + 1;
                    while (Next() != '\n')
                        /* skip to eol*/;

                    var comment = _Factory.NewToken(
                        EPiToken.Comment,
                        new Slice(this, start, _offset));
                    _Tokens.Add(comment);
                    return true;
                }

                return Add(EPiToken.Divide);
            }

            LexError($"Unrecognised '{current}'");

            return false;
        }

        private bool AddQuotedOperation()
        {
            Next();
            var num = int.Parse(Gather(char.IsDigit).Text);
            var op = (EOperation) num;
            return _opToToken.TryGetValue(op, out var tok) && Add(tok);
        }

        protected override void AddKeywordOrIdent(Slice slice)
            => _Tokens.Add(_KeyWords.TryGetValue(slice.Text, out var tok)
            ? _Factory.NewToken(tok, slice)
            : _Factory.NewTokenIdent(slice));

        protected override void Terminate()
        {
        }
    }
}


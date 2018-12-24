namespace Diver.Language.PiLang
{
    public class PiLexer
        : LexerCommon<EToken, PiToken, PiTokenFactory>
    {
        public PiLexer(string input)
            : base(input)
        {
            var factory = new PiTokenFactory();
            factory.SetLexer(this);
        }

        protected override void AddKeyWords()
        {
            _keyWords.Add("if", EToken.If);
            _keyWords.Add("ife", EToken.IfElse);
            _keyWords.Add("for", EToken.For);
            _keyWords.Add("true", EToken.True);
            _keyWords.Add("false", EToken.False);
            _keyWords.Add("self", EToken.Self);
            _keyWords.Add("while", EToken.While);
            _keyWords.Add("assert", EToken.Assert);
            _keyWords.Add("div", EToken.Divide);
            _keyWords.Add("rho", EToken.ToRho);
            _keyWords.Add("rho{", EToken.ToRhoSequence);
            _keyWords.Add("not", EToken.Not);
            _keyWords.Add("and", EToken.And);
            _keyWords.Add("or", EToken.Or);
            _keyWords.Add("xor", EToken.Xor);
            _keyWords.Add("exists", EToken.Exists);
            _keyWords.Add("drop", EToken.Drop);
            _keyWords.Add("dup", EToken.Dup);
            _keyWords.Add("over", EToken.Over);
            _keyWords.Add("swap", EToken.Swap);
            _keyWords.Add("rot", EToken.Rot);
            _keyWords.Add("rotn", EToken.RotN);
            _keyWords.Add("toarray", EToken.ToArray);
            _keyWords.Add("gc", EToken.GarbageCollect);
            _keyWords.Add("clear", EToken.Clear);
            _keyWords.Add("cd", EToken.ChangeFolder);
            _keyWords.Add("pwd", EToken.PrintFolder);
            _keyWords.Add("type", EToken.GetType);
            _keyWords.Add("size", EToken.Size);
            _keyWords.Add("depth", EToken.Depth);
            _keyWords.Add("new", EToken.New);
            _keyWords.Add("dropn", EToken.DropN);
            _keyWords.Add("tolist", EToken.ToList);
            _keyWords.Add("tomap", EToken.ToMap);
            _keyWords.Add("toset", EToken.ToSet);
            _keyWords.Add("expand", EToken.Expand);
            _keyWords.Add("noteq", EToken.NotEquiv);
            _keyWords.Add("lls", EToken.Contents);
            _keyWords.Add("ls", EToken.GetContents);
            _keyWords.Add("freeze", EToken.Freeze);
            _keyWords.Add("thaw", EToken.Thaw);
        }

        protected override bool NextToken()
        {
            char current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current))
                return IdentOrKeyword();

            if (char.IsDigit(current))
                return AddSlice(EToken.Int, Gather(char.IsDigit));

            switch (current)
            {
            case Pathname.Quote: return Add(EToken.Quote);
            case '{': return Add(EToken.OpenBrace);
            case '}': return Add(EToken.CloseBrace);
            case '(': return Add(EToken.OpenParan);
            case ')': return Add(EToken.CloseParan);
            case ':': return Add(EToken.Colon);
            case ' ': return AddSlice(EToken.Whitespace, Gather(IsSpaceChar));
            case '@': return Add(EToken.Retrieve);
            case ',': return Add(EToken.Comma);
            case '#': return Add(EToken.Store);
            case '*': return Add(EToken.Multiply);
            case '[': return Add(EToken.OpenSquareBracket);
            case ']': return Add(EToken.CloseSquareBracket);
            case '=': return AddIfNext('=', EToken.Equiv, EToken.Assign);
            case '!': return Add(EToken.Replace);
            case '&': return Add(EToken.Suspend);
            case '|': return AddIfNext('|', EToken.Or, EToken.BitOr);
            case '<': return AddIfNext('=', EToken.LessEquiv, EToken.LessEquiv);
            case '>': return AddIfNext('=', EToken.GreaterEquiv, EToken.Greater);
            case '"': return LexString(); 
            case '\t': return Add(EToken.Tab);
            case '\n': return Add(EToken.NewLine);
            case '-':
                if (Peek() == '-')
                    return AddTwoCharOp(EToken.Decrement);
                if (Peek() == '=')
                    return AddTwoCharOp(EToken.MinusAssign);
                return Add(EToken.Minus);

            case '.':
                if (Peek() == '.')
                {
                    Next();
                    if (Peek() == '.')
                    {
                        Next();
                        return Add(EToken.Resume, 3);
                    }
                    return Fail("Two dots doesn't work");
                }
                return Add(EToken.Dot);

            case '+':
                if (Peek() == '+')
                    return AddTwoCharOp(EToken.Increment);
                if (Peek() == '=')
                    return AddTwoCharOp(EToken.PlusAssign);
                return Add(EToken.Plus);

            case '/':
                if (Peek() == '/')
                {
                    Next();
                    int start = _offset + 1;
                    while (Next() != '\n')
                        ;

                    var comment = _factory.NewToken(
                        EToken.Comment,
                        new Slice(this, start, _offset));
                    _tokens.Add(comment);
                    return true;
                }

                //return LexError("/ is not a valid PiToken");//Add(EToken.Divide);
                return Add(EToken.Separator);
            }

            LexError("Unrecognised %c");

            return false;
        }

        private bool IdentOrKeyword()
        {
            var begin = _offset;
            Next();
            while (char.IsLetterOrDigit(Current()))
                Next();
            _tokens.Add(_factory.NewTokenIdent(new Slice(this, begin, _offset)));
            return true;
        }

        private bool IsSpaceChar(char arg)
        {
            return char.IsWhiteSpace(arg);
        }

        protected override void Terminate()
        {
        }
    }
}

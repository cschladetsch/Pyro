namespace Diver.Language
{
    public class PiLexer
        : LexerCommon<EPiToken, PiToken, PiTokenFactory>
    {
        public PiLexer(string input)
            : base(input)
        {
            var factory = new PiTokenFactory();
            factory.SetLexer(this);
        }

        protected override void AddKeyWords()
        {
            _keyWords.Add("break", EPiToken.Break);
            _keyWords.Add("ife", EPiToken.IfElse);
            _keyWords.Add("for", EPiToken.For);
            _keyWords.Add("true", EPiToken.True);
            _keyWords.Add("false", EPiToken.False);
            _keyWords.Add("self", EPiToken.Self);
            _keyWords.Add("while", EPiToken.While);
            _keyWords.Add("assert", EPiToken.Assert);
            _keyWords.Add("div", EPiToken.Divide);
            _keyWords.Add("rho", EPiToken.ToRho);
            _keyWords.Add("rho{", EPiToken.ToRhoSequence);
            _keyWords.Add("not", EPiToken.Not);
            _keyWords.Add("and", EPiToken.And);
            _keyWords.Add("or", EPiToken.Or);
            _keyWords.Add("xor", EPiToken.Xor);
            _keyWords.Add("exists", EPiToken.Exists);
            _keyWords.Add("drop", EPiToken.Drop);
            _keyWords.Add("dup", EPiToken.Dup);
            _keyWords.Add("over", EPiToken.Over);
            _keyWords.Add("swap", EPiToken.Swap);
            _keyWords.Add("rot", EPiToken.Rot);
            _keyWords.Add("rotn", EPiToken.RotN);
            _keyWords.Add("gc", EPiToken.GarbageCollect);
            _keyWords.Add("clear", EPiToken.Clear);
            _keyWords.Add("cd", EPiToken.ChangeFolder);
            _keyWords.Add("pwd", EPiToken.PrintFolder);
            _keyWords.Add("type", EPiToken.GetType);
            _keyWords.Add("depth", EPiToken.Depth);
            _keyWords.Add("new", EPiToken.New);
            _keyWords.Add("dropn", EPiToken.DropN);
            _keyWords.Add("noteq", EPiToken.NotEquiv);
            _keyWords.Add("lls", EPiToken.Contents);
            _keyWords.Add("ls", EPiToken.GetContents);
            _keyWords.Add("freeze", EPiToken.Freeze);
            _keyWords.Add("thaw", EPiToken.Thaw);
            _keyWords.Add("size", EPiToken.Size);
            _keyWords.Add("tomap", EPiToken.ToMap);
            _keyWords.Add("toset", EPiToken.ToSet);
            _keyWords.Add("tolist", EPiToken.ToList);
            _keyWords.Add("toarray", EPiToken.ToArray);
            _keyWords.Add("expand", EPiToken.Expand);
            _keyWords.Add("remove", EPiToken.Remove);
            _keyWords.Add("push_front", EPiToken.PushFront);
            _keyWords.Add("push_back", EPiToken.PushFront);
            _keyWords.Add("back", EPiToken.GetBack);
            _keyWords.Add("at", EPiToken.At);
            _keyWords.Add("insert", EPiToken.Insert);
            _keyWords.Add("has", EPiToken.Has);
        }

        protected override bool NextToken()
        {
            char current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            if (char.IsDigit(current))
                return AddSlice(EPiToken.Int, Gather(char.IsDigit));

            switch (current)
            {
            case Pathname.Quote: return Add(EPiToken.Quote);
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
            case '!': return Add(EPiToken.Replace);
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
                    int start = _offset + 1;
                    while (Next() != '\n')
                        ;

                    var comment = _factory.NewToken(
                        EPiToken.Comment,
                        new Slice(this, start, _offset));
                    _tokens.Add(comment);
                    return true;
                }

                //return LexError("/ is not a valid PiToken");//Add(EPiToken.Divide);
                return Add(EPiToken.Separator);
            }

            LexError($"Unrecognised '{current}'");

            return false;
        }

        private bool IdentOrKeyword()
        {
            AddKeywordOrIdent(GatherIdent());
            return true;
        }

        private void AddKeywordOrIdent(Slice slice)
        {
            if (_keyWords.TryGetValue(slice.Text, out EPiToken tok))
                _tokens.Add(_factory.NewToken(tok, slice));
            else
                _tokens.Add(_factory.NewTokenIdent(slice));
        }

        bool IsValidIdentGlyph(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }

        private Slice GatherIdent()
        {
            var begin = _offset;
            Next();
            while (IsValidIdentGlyph(Current()))
                Next();
            return new Slice(this, begin, _offset);
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

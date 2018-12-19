namespace Diver.PiLang
{
    public class Lexer : LexerCommon<EToken, Token, TokenFactory>
    {
        public Lexer(string input, TokenFactory factory) : base(input, factory)
        {
        }

        protected override void AddKeyWords()
        {
            _keyWords["if"] = EToken.If;
            _keyWords["ife"] = EToken.IfElse;
            _keyWords["for"] = EToken.For;
            _keyWords["true"] = EToken.True;
            _keyWords["false"] = EToken.False;
            _keyWords["self"] = EToken.Self;
            _keyWords["while"] = EToken.While;
            _keyWords["assert"] = EToken.Assert;
            _keyWords["div"] = EToken.Divide;
            _keyWords["rho"] = EToken.ToRho;
            _keyWords["rho{"] = EToken.ToRhoSequence;

            _keyWords["not"] = EToken.Not;
            _keyWords["and"] = EToken.And;
            _keyWords["or"] = EToken.Or;
            _keyWords["xor"] = EToken.Xor;
            _keyWords["exists"] = EToken.Exists;

            _keyWords["drop"] = EToken.Drop;
            _keyWords["dup"] = EToken.Dup;
            _keyWords["over"] = EToken.Over;
            _keyWords["swap"] = EToken.Swap;
            _keyWords["rot"] = EToken.Rot;
            _keyWords["rotn"] = EToken.RotN;
            _keyWords["toarray"] = EToken.ToArray;
            _keyWords["gc"] = EToken.GarbageCollect;
            _keyWords["clear"] = EToken.Clear;
            _keyWords["expand"] = EToken.Expand;
            _keyWords["cd"] = EToken.ChangeFolder;
            _keyWords["pwd"] = EToken.PrintFolder;
            _keyWords["type"] = EToken.GetType;
            _keyWords["size"] = EToken.Size;
            _keyWords["depth"] = EToken.Depth;
            _keyWords["new"] = EToken.New;
            _keyWords["dropn"] = EToken.DropN;

            _keyWords["toarray"] = EToken.ToArray;
            _keyWords["tolist"] = EToken.ToList;
            _keyWords["tomap"] = EToken.ToMap;
            _keyWords["toset"] = EToken.ToSet;

            _keyWords["div"] = EToken.Divide;
            _keyWords["mul"] = EToken.Mul;
            
            _keyWords["expand"] = EToken.Expand;
            _keyWords["noteq"] = EToken.NotEquiv;
            _keyWords["lls"] = EToken.Contents;
            _keyWords["ls"] = EToken.GetContents;
            _keyWords["freeze"] = EToken.Freeze;
            _keyWords["thaw"] = EToken.Thaw;
        }

        protected override bool NextToken()
        {
            char current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current))
                return PathnameOrKeyword();

            if (char.IsDigit(current))
                return Add(EToken.Int, Gather(char.IsDigit));

            switch (current)
            {
            //case '\'': return PathnameOrKeyword();
            case '{': return Add(EToken.OpenBrace);
            case '}': return Add(EToken.CloseBrace);
            case '(': return Add(EToken.OpenParan);
            case ')': return Add(EToken.CloseParan);
            case ':': return Add(EToken.Colon);
            case ' ': return Add(EToken.Whitespace, Gather(IsSpaceChar));
            case '@': return Add(EToken.Lookup);
            case ',': return Add(EToken.Comma);
            case '#': return Add(EToken.Store);
            case '*': return Add(EToken.Mul);
            case '[': return Add(EToken.OpenSquareBracket);
            case ']': return Add(EToken.CloseSquareBracket);
            case '=': return AddIfNext('=', EToken.Equiv, Enum::Assign);
            case '!': return Add(EToken.Replace);
            case '&': return Add(EToken.Suspend);
            case '|': return AddIfNext('|', EToken.Or, Enum::BitOr);
            case '<': return AddIfNext('=', EToken.LessEquiv, Enum::Less);
            case '>': return AddIfNext('=', EToken.GreaterEquiv, Enum::Greater);
            case '"': return LexString(); // "comment to unfuck Visual Studio Code's syntax hilighter
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
                return Add(EToken.Self);

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
                    const int start = offset;
                    while (Next() != '\n')
                        ;

                    Add(Token(EToken.Comment, *this, lineNumber, Slice(start, offset)));
                    Next();
                    return true;
                }
                return PathnameOrKeyword();
            }

            LexError("Unrecognised %c");

            return false;
        }r

        protected override void Terminate()
        {
        }
    }
}

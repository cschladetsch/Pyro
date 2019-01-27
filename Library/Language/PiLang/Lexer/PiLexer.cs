using System.Collections.Generic;
using Pryo;
using Pyro.Exec;
using Pyro.Language.Impl;

namespace Pyro.Language.Lexer
{
    public partial class PiLexer
        : LexerCommon<EPiToken, PiToken, PiTokenFactory>, ILexer
    {
        public PiLexer(string input)
            : base(input)
        {
            //var factory = new PiTokenFactory();
            //factory.SetLexer(this);
            CreateOpToToken();
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
                    int start = _offset + 1;
                    while (Next() != '\n')
                        ;

                    var comment = _Factory.NewToken(
                        EPiToken.Comment,
                        new Slice(this, start, _offset));
                    _Tokens.Add(comment);
                    return true;
                }

                return Add(EPiToken.Separator);
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
        {
            _Tokens.Add(_KeyWords.TryGetValue(slice.Text, out var tok)
                ? _Factory.NewToken(tok, slice)
                : _Factory.NewTokenIdent(slice));
        }

        protected override void Terminate()
        {
        }

        public static Dictionary<EOperation, EPiToken> _opToToken = new Dictionary<EOperation, EPiToken>();
    }

    public partial class PiLexer
    {
        protected override void AddKeyWords()
        {
            _KeyWords.Add("break", EPiToken.Break);
            _KeyWords.Add("if", EPiToken.If);
            _KeyWords.Add("ife", EPiToken.IfElse);
            _KeyWords.Add("for", EPiToken.For);
            _KeyWords.Add("true", EPiToken.True);
            _KeyWords.Add("false", EPiToken.False);
            _KeyWords.Add("self", EPiToken.Self);
            _KeyWords.Add("while", EPiToken.While);
            _KeyWords.Add("assert", EPiToken.Assert);
            _KeyWords.Add("div", EPiToken.Divide);
            _KeyWords.Add("rho", EPiToken.ToRho);
            _KeyWords.Add("rho{", EPiToken.ToRhoSequence);
            _KeyWords.Add("not", EPiToken.Not);
            _KeyWords.Add("and", EPiToken.And);
            _KeyWords.Add("or", EPiToken.Or);
            _KeyWords.Add("xor", EPiToken.Xor);
            _KeyWords.Add("exists", EPiToken.Exists);
            _KeyWords.Add("drop", EPiToken.Drop);
            _KeyWords.Add("dup", EPiToken.Dup);
            _KeyWords.Add("over", EPiToken.Over);
            _KeyWords.Add("swap", EPiToken.Swap);
            _KeyWords.Add("rot", EPiToken.Rot);
            _KeyWords.Add("pick", EPiToken.Pick);
            _KeyWords.Add("rotn", EPiToken.RotN);
            _KeyWords.Add("gc", EPiToken.GarbageCollect);
            _KeyWords.Add("clear", EPiToken.Clear);
            _KeyWords.Add("cd", EPiToken.ChangeFolder);
            _KeyWords.Add("pwd", EPiToken.PrintFolder);
            _KeyWords.Add("type", EPiToken.GetType);
            _KeyWords.Add("depth", EPiToken.Depth);
            _KeyWords.Add("new", EPiToken.New);
            _KeyWords.Add("dropn", EPiToken.DropN);
            _KeyWords.Add("noteq", EPiToken.NotEquiv);
            _KeyWords.Add("lls", EPiToken.Contents);
            _KeyWords.Add("ls", EPiToken.GetContents);
            _KeyWords.Add("freeze", EPiToken.Freeze);
            _KeyWords.Add("thaw", EPiToken.Thaw);
            _KeyWords.Add("size", EPiToken.Size);
            _KeyWords.Add("tomap", EPiToken.ToMap);
            _KeyWords.Add("toset", EPiToken.ToSet);
            _KeyWords.Add("tolist", EPiToken.ToList);
            _KeyWords.Add("toarray", EPiToken.ToArray);
            _KeyWords.Add("expand", EPiToken.Expand);
            _KeyWords.Add("remove", EPiToken.Remove);
            _KeyWords.Add("push_front", EPiToken.PushFront);
            _KeyWords.Add("push_back", EPiToken.PushBack);
            _KeyWords.Add("back", EPiToken.GetBack);
            _KeyWords.Add("at", EPiToken.At);
            _KeyWords.Add("insert", EPiToken.Insert);
            _KeyWords.Add("has", EPiToken.Has);
            _KeyWords.Add("debug_datastack", EPiToken.DebugPrintDataStack);
            _KeyWords.Add("set_float_precision", EPiToken.SetFloatPrecision);
        }

        private void CreateOpToToken()
        {
            //_opToToken[EOperation.Nop] = EPiToken.Nop;
            //_opToToken[EOperation.HasType] = EPiToken.HasType;
            _opToToken[EOperation.GarbageCollect] = EPiToken.GarbageCollect;
            _opToToken[EOperation.Plus] = EPiToken.Plus;
            _opToToken[EOperation.Minus] = EPiToken.Minus;
            _opToToken[EOperation.Multiply] = EPiToken.Multiply;
            _opToToken[EOperation.Divide] = EPiToken.Divide;
            //_opToToken[EOperation.Modulo] = EPiToken.Modulo;
            _opToToken[EOperation.Store] = EPiToken.Store;
            _opToToken[EOperation.Has] = EPiToken.Has;
            _opToToken[EOperation.Retrieve] = EPiToken.Retrieve;
            _opToToken[EOperation.Assign] = EPiToken.Assign;
            //_opToToken[EOperation.GetPath] = EPiToken.GetPath;
            _opToToken[EOperation.Suspend] = EPiToken.Suspend;
            _opToToken[EOperation.Resume] = EPiToken.Resume;
            _opToToken[EOperation.Replace] = EPiToken.Replace;
            _opToToken[EOperation.Assert] = EPiToken.Assert;
            _opToToken[EOperation.Write] = EPiToken.Write;
            _opToToken[EOperation.WriteLine] = EPiToken.WriteLine;
            _opToToken[EOperation.If] = EPiToken.If;
            _opToToken[EOperation.IfElse] = EPiToken.IfElse;
            //_opToToken[EOperation.StackToList] = EPiToken.StackToList;
            //_opToToken[EOperation.ListToStack] = EPiToken.ListToStack;
            _opToToken[EOperation.Depth] = EPiToken.Depth;
            _opToToken[EOperation.Dup] = EPiToken.Dup;
            _opToToken[EOperation.Clear] = EPiToken.Clear;
            _opToToken[EOperation.Swap] = EPiToken.Swap;
            _opToToken[EOperation.Break] = EPiToken.Break;
            _opToToken[EOperation.Rot] = EPiToken.Rot;
            //_opToToken[EOperation.Roll] = EPiToken.Roll;
            _opToToken[EOperation.RotN] = EPiToken.RotN;
            //_opToToken[EOperation.RollN] = EPiToken.RollN;
            _opToToken[EOperation.Pick] = EPiToken.Pick;
            _opToToken[EOperation.Over] = EPiToken.Over;
            _opToToken[EOperation.Freeze] = EPiToken.Freeze;
            _opToToken[EOperation.Thaw] = EPiToken.Thaw;
            //_opToToken[EOperation.FreezeText] = EPiToken.FreezeText;
            //_opToToken[EOperation.ThawText] = EPiToken.ThawText;
            //_opToToken[EOperation.FreezeYaml] = EPiToken.FreezeYaml;
            //_opToToken[EOperation.ThawYaml] = EPiToken.ThawYaml;
            _opToToken[EOperation.Not] = EPiToken.Not;
            _opToToken[EOperation.Equiv] = EPiToken.Equiv;
            _opToToken[EOperation.LogicalAnd] = EPiToken.And;
            _opToToken[EOperation.LogicalOr] = EPiToken.Or;
            _opToToken[EOperation.LogicalXor] = EPiToken.Xor;
            _opToToken[EOperation.Less] = EPiToken.Less;
            _opToToken[EOperation.Greater] = EPiToken.Greater;
            _opToToken[EOperation.GreaterOrEquiv] = EPiToken.GreaterEquiv;
            _opToToken[EOperation.LessOrEquiv] = EPiToken.LessEquiv;
            _opToToken[EOperation.NotEquiv] = EPiToken.NotEquiv;
            _opToToken[EOperation.Expand] = EPiToken.Expand;
            _opToToken[EOperation.ToArray] = EPiToken.ToArray;
            _opToToken[EOperation.ToMap] = EPiToken.ToMap;
            _opToToken[EOperation.ToSet] = EPiToken.ToSet;
            //_opToToken[EOperation.ToPair] = EPiToken.ToPair;
            _opToToken[EOperation.Size] = EPiToken.Size;
            _opToToken[EOperation.GetBack] = EPiToken.GetBack;
            _opToToken[EOperation.PushBack] = EPiToken.PushBack;
            _opToToken[EOperation.PushFront] = EPiToken.PushFront;
            _opToToken[EOperation.ToList] = EPiToken.ToList;
            _opToToken[EOperation.Remove] = EPiToken.Remove;
            _opToToken[EOperation.Insert] = EPiToken.Insert;
            _opToToken[EOperation.At] = EPiToken.At;
            _opToToken[EOperation.DebugPrintDataStack] = EPiToken.DebugPrintDataStack;
            _opToToken[EOperation.DebugPrintContextStack] = EPiToken.DebugPrintContextStack;
            _opToToken[EOperation.DebugPrint] = EPiToken.DebugPrint;
            _opToToken[EOperation.DebugPrintContinuation] = EPiToken.DebugPrintContinuation;
            //_opToToken[EOperation.DebugSetLevel] = EPiToken.DebugSetLevel;
            _opToToken[EOperation.SetFloatPrecision] = EPiToken.SetFloatPrecision;
            _opToToken[EOperation.Self] = EPiToken.Self;
            _opToToken[EOperation.GetMember] = EPiToken.GetMember;
            //_opToToken[EOperation.SetMember] = EPiToken.SetMember;
            //_opToToken[EOperation.SetMemberValue] = EPiToken.SetMemberValue;
            //_opToToken[EOperation.ForEachIn] = EPiToken.ForEachIn;
            //_opToToken[EOperation.ForLoop] = EPiToken.ForLoop;
            _opToToken[EOperation.Drop] = EPiToken.Drop;
            _opToToken[EOperation.DropN] = EPiToken.DropN;
        }
    }
}

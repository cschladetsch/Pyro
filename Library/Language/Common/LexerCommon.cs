using System;
using System.Collections.Generic;
using System.Text;

namespace Diver.Language
{
    public abstract class LexerCommon<TEnum, TToken, TTokenFactory> 
        : LexerBase, ILexerCommon<TToken>
        where TToken : class, ITokenBase<TEnum>, new()
        where TTokenFactory : class, ITokenFactory<TEnum, TToken>, new()
    {
        public IList<TToken> Tokens => _tokens;

        protected LexerCommon(string input) : base(input)
        {
            _factory.SetLexer(this);
        }

        public bool Process()
        {
            AddKeyWords();
            CreateLines();
            return Run();
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var tok in _tokens)
            {
                str.Append($"{tok}, ");
            }
            return str.ToString();
        }

        protected virtual void AddKeyWords()
        {
        }

        protected virtual bool NextToken()
        {
            return false;
        }

        protected virtual void Terminate()
        {
        }

        protected bool Run()
        {
            _offset = 0;
            _lineNumber = 0;

            while (!Failed && NextToken())
                ;

            Terminate();

            return !Failed;
        }

        protected TToken LexAlpha()
        {
            TToken tok = _factory.NewTokenIdent(Gather(char.IsLetter));
            TEnum en;
            if (_keyWords.TryGetValue(tok.ToString(), out en))
                tok.Type = en;

            return tok;
        }

        protected override void LexError(string fmt, params object[] args)
        {
            _error = string.Format(fmt, args);
        }

        protected override void AddStringToken(Slice slice)
        {
            _tokens.Add(_factory.NewTokenString(slice));
        }

        protected bool AddSlice(TEnum type, Slice slice)
        {
            _tokens.Add(_factory.NewToken(type, slice));
            return true;
        }

        protected bool Add(TEnum type, int len = 1)
        {
            AddSlice(type, new Slice(this, _offset, _offset + len));
            while (len-- > 0)
                Next();

            return true;
        }

        protected bool AddIfNext(char ch, TEnum thenType, TEnum elseType)
        {
            if (Peek() == ch)
            {
                Add(thenType, 2);
                Next();
                return true;
            }

            return Add(elseType, 1);
        }

        protected bool AddTwoCharOp(TEnum ty)
        {
            Add(ty, 2);
            Next();

            return true;
        }

        protected bool AddThreeCharOp(TEnum ty)
        {
            Add(ty, 3);
            Next();
            Next();

            return true;
        }

        protected bool LexError(string text)
        {
            return Fail(CreateErrorMessage(_factory.NewEmptyToken(new Slice(this, _offset, _offset)), text, Current()));
        }

        public string CreateErrorMessage(TToken tok, string fmt, params object[] args)
        {
            var buff = $"({tok.LineNumber}):[{tok.Slice.Start}]: {string.Format(fmt, args)}";
            int beforeContext = 2;
            int afterContext = 2;

            var lex = tok.Lexer;
            int start = Math.Max(0, tok.LineNumber - beforeContext);
            int end = Math.Min(lex.Lines.Count - 1, tok.LineNumber + afterContext);

            var str = new StringBuilder();
            str.AppendLine(buff);
            for (int n = start; n <= end; ++n)
            {
                foreach (var ch in lex.GetLine(n))
                {
                    if (ch == '\t')
                        str.Append("    ");
                    else
                        str.Append(ch);
                }

                if (n == tok.LineNumber)
                {
                    for (int ch = 0; ch < (int)lex.GetLine(n).Length; ++ch)
                    {
                        if (ch == tok.Slice.Start)
                        {
                            str.Append('^');
                            break;
                        }

                        var c = lex.GetLine(tok.LineNumber)[ch];
                        if (c == '\t')
                            str.Append("    ");
                        else
                            str.Append(' ');
                    }
                }
            }

            return str.ToString();
        }

        protected List<TToken> _tokens = new List<TToken>();
        protected Dictionary<string, TEnum> _keyWords = new Dictionary<string, TEnum>();
        protected TTokenFactory _factory = new TTokenFactory();
    }
}

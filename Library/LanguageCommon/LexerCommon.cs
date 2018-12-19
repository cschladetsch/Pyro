using System;
using System.Collections.Generic;
using System.Text;

namespace Diver
{
    public abstract class LexerCommon<TEnum, TToken, TTokenFactory> 
        : LexerBase
        where TToken : class, ITokenBase<TEnum>, new()
        where TTokenFactory : ITokenFactory<TEnum, TToken>
    {
        public IList<TToken> Tokens => _tokens;

        protected LexerCommon(string input, TTokenFactory factory, IRegistry reg) : base(input, reg)
        {
            _factory = factory;
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
            TToken tok = _factory.NewTokenIdent(this, _lineNumber, Gather((c) => char.IsLetter(c)));
            TEnum en;
            if (_keyWords.TryGetValue(tok.ToString(), out en))
                tok.Type = en;

            return tok;
        }

        protected override void LexError(string fmt, params object[] args)
        {
            _error = string.Format(fmt, args);
        }

        protected override void AddStringToken(int lineNumber, Slice slice)
        {
            _tokens.Add(_factory.NewTokenString(this, lineNumber, slice));
        }

        protected bool Add(TToken token)
        {
            _tokens.Add(token);
            return true;
        }

        protected bool Add(TEnum type, Slice slice)
        {
            _tokens.Add(_factory.NewToken(this, type,_lineNumber, slice));
            return true;
        }

        bool Add(TEnum type, int len = 1)
        {
            Add(type, new Slice(_offset, _offset + len));
            while (len-- > 0)
                Next();

            return true;
        }

        protected bool AddIfNext(char ch, TEnum thentype, TEnum elseType)
        {
            if (Peek() == ch)
            {
                Next();
                return Add(thentype, 2);
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
            return Fail(CreateErrorMessage(_factory.NewEmptyToken(this, _lineNumber, new Slice(_offset, _offset)), text, Current()));
        }

        static string CreateErrorMessage(TToken tok, string fmt, params object[] args)
        {
            var buff = $"({tok.LineNumber}):[{tok.Slice.Start}: {string.Format(fmt, args)}";
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
        protected Dictionary<string, TEnum> _keyWords;
        protected TTokenFactory _factory;
    }
}

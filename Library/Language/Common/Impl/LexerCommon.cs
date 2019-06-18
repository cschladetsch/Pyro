using System;
using System.Text;
using System.Collections.Generic;

namespace Pyro.Language.Impl
{
    /// <inheritdoc cref="LexerBase" />
    /// <summary>
    /// Common to all Lexers
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type for tokens in the language</typeparam>
    /// <typeparam name="TToken">The token type to use</typeparam>
    /// <typeparam name="TTokenFactory">How to make Tokens</typeparam>
    public abstract class LexerCommon<TEnum, TToken, TTokenFactory>
        : LexerBase
        , ILexerCommon<TToken>
        where TToken : class, ITokenBase<TEnum>, new()
        where TTokenFactory : class, ITokenFactory<TEnum, TToken>, new()
    {
        public IList<TToken> Tokens => _Tokens;

        protected List<TToken> _Tokens = new List<TToken>();
        protected Dictionary<string, TEnum> _KeyWords = new Dictionary<string, TEnum>();
        protected Dictionary<TEnum, string> _KeyWordsInvert = new Dictionary<TEnum, string>();
        protected TTokenFactory _Factory = new TTokenFactory();

        protected LexerCommon(string input)
            : base(input)
            => _Factory.SetLexer(this);

        protected override void LexError(string fmt, params object[] args) => Error = string.Format(fmt, args);
        protected override void AddStringToken(Slice slice) => _Tokens.Add(_Factory.NewTokenString(slice));
        protected virtual bool NextToken() => false;
        protected virtual void Terminate() { }
        protected virtual void AddKeyWords() { }

        public TEnum StringToEnum(string str)
            => _KeyWords.TryGetValue(str, out var e) ? e : default;

        public string EnumToString(TEnum e)
            => _KeyWordsInvert.TryGetValue(e, out var str) ? str : e.ToString();

        public bool Process()
        {
            AddKeyWords();
            CreateLines();
            return Run();
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var tok in _Tokens)
                str.Append($"{tok}, ");

            return str.ToString();
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
            var tok = _Factory.NewTokenIdent(Gather(char.IsLetter));
            if (_KeyWords.TryGetValue(tok.ToString(), out var en))
                tok.Type = en;

            return tok;
        }

        protected bool AddSlice(TEnum type, Slice slice)
        {
            _Tokens.Add(_Factory.NewToken(type, slice));
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
            if (Peek() != ch)
                return Add(elseType, 1);

            Add(thenType, 2);
            Next();
            return true;
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
            => Fail(CreateErrorMessage(_Factory.NewEmptyToken(new Slice(this, _offset, _offset)), text, Current()));

        public string CreateErrorMessage(TToken tok, string fmt, params object[] args)
        {
            var buff = $"({tok.LineNumber}):[{tok.Slice.Start}]: {string.Format(fmt, args)}";
            const int beforeContext = 2;
            const int afterContext = 2;

            var lex = tok.Lexer;
            var start = Math.Max(0, tok.LineNumber - beforeContext);
            var end = Math.Min(lex.Lines.Count - 1, tok.LineNumber + afterContext);

            var str = new StringBuilder();
            str.AppendLine(buff);
            for (var n = start; n <= end; ++n)
            {
                foreach (var ch in lex.GetLine(n))
                {
                    if (ch == '\t')
                        str.Append("    ");
                    else
                        str.Append(ch);
                }

                if (n != tok.LineNumber)
                    continue;

                for (var ch = 0; ch < lex.GetLine(n).Length; ++ch)
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

            return str.ToString();
        }
    }
}


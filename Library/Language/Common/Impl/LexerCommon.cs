using System;
using System.Text;
using System.Collections.Generic;

namespace Pyro.Language.Impl {
    public abstract class LexerCommon<TEnum, TToken, TTokenFactory>
        : LexerBase
            , ILexerCommon<TToken>
        where TToken : class, ITokenBase<TEnum>, new()
        where TTokenFactory : class, ITokenFactory<TEnum, TToken>, new() {

        public TToken ErrorToken;

        public IList<TToken> Tokens => _Tokens;

        protected TTokenFactory _Factory = new TTokenFactory();

        protected readonly Dictionary<string, TEnum> _keyWords = new Dictionary<string, TEnum>();

        protected readonly Dictionary<TEnum, string> _KeyWordsInvert = new Dictionary<TEnum, string>();

        protected List<TToken> _Tokens = new List<TToken>();

        protected LexerCommon(string input)
            : base(input) {
        }

        public string CreateErrorMessage(TToken tok, string fmt, params object[] args) {
            ErrorToken = tok;
            var buff = $"({tok.LineNumber}):[{tok.Slice.Start}]: {string.Format(fmt, args)}";
            const int beforeContext = 2;
            const int afterContext = 2;

            var lex = tok.Lexer;
            var start = Math.Max(0, tok.LineNumber - beforeContext);
            var end = Math.Min(lex.Lines.Count - 1, tok.LineNumber + afterContext);

            var str = new StringBuilder();
            str.AppendLine(buff);
            for (var n = start; n <= end; ++n) {
                foreach (var ch in lex.GetLine(n))
                    if (ch == '\t') {
                        str.Append("    ");
                    }
                    else {
                        str.Append(ch);
                    }

                if (n != tok.LineNumber) {
                    continue;
                }

                for (var ch = 0; ch < lex.GetLine(n).Length; ++ch) {
                    if (ch == tok.Slice.Start) {
                        str.Append('^');
                        break;
                    }

                    var c = lex.GetLine(tok.LineNumber)[ch];
                    if (c == '\t') {
                        str.Append("    ");
                    }
                    else {
                        str.Append(' ');
                    }
                }
            }

            return str.ToString();
        }

        public TEnum StringToEnum(string str) {
            return _keyWords.TryGetValue(str, out var e) ? e : default;
        }

        public string EnumToString(TEnum e) {
            return _KeyWordsInvert.TryGetValue(e, out var str) ? str : e.ToString();
        }

        public bool Process() {
            AddKeyWords();
            CreateLines();
            return Run() && !Failed;
        }

        public override string ToString() {
            var str = new StringBuilder();
            foreach (var tok in _Tokens)
                str.Append($"{tok}, ");

            return str.ToString();
        }

        protected override void LexError(string fmt, params object[] args) {
            Error = string.Format(fmt, args);
        }

        protected override void AddStringToken(Slice slice) {
            _Tokens.Add(_Factory.NewTokenString(slice));
        }

        protected virtual bool NextToken() {
            return false;
        }

        protected virtual void Terminate() {
        }

        protected virtual void AddKeyWords() {
        }

        public override bool Run() {
            _offset = 0;
            _lineNumber = 0;

            while (!Failed && NextToken()) {
            }

            Terminate();

            return !Failed;
        }

        protected TToken LexAlpha() {
            var tok = _Factory.NewTokenIdent(Gather(char.IsLetter));
            if (_keyWords.TryGetValue(tok.ToString(), out var en)) {
                tok.Type = en;
            }

            return tok;
        }

        protected bool AddSlice(TEnum type, Slice slice) {
            _Tokens.Add(_Factory.NewToken(type, slice));
            return true;
        }

        protected bool Add(TEnum type, int len = 1) {
            AddSlice(type, new Slice(this, _offset, _offset + len));
            while (len-- > 0)
                Next();

            return true;
        }

        protected bool AddIfNext(char ch, TEnum thenType, TEnum elseType) {
            if (Peek() != ch) {
                return Add(elseType);
            }

            Add(thenType, 2);
            Next();
            return true;
        }

        protected bool AddTwoCharOp(TEnum ty) {
            return Add(ty, 2);
        }

        protected bool AddThreeCharOp(TEnum ty) {
            return Add(ty, 3);
        }

        protected bool LexError(string text) {
            return Fail(
                CreateErrorMessage(
                    _Factory.NewEmptyToken(new Slice(this, _offset, _offset)), text, Current()));
        }
    }
}


using System.Collections.Generic;
using System.Text;
using Diver.Core;

namespace Diver
{
    public interface ITokenFactory<TEnum, TToken>
        where TToken : class, new()
    {
        TToken NewToken(LexerBase lexer, TEnum en, int lineNumber, Slice slice);
        TToken NewToken(LexerBase lexer, Slice slice);
        TToken NewTokenIdent(LexerBase lexer, int lineNumber, Slice slice);
        TToken NewTokenString(LexerBase lexer, int lineNumber, Slice slice);
        TToken NewEmptyToken(LexerBase lexer, int lineNumber, Slice slice);
    }

    public abstract class LexerCommon<TEnum, TToken, TTokenFactory> 
        : LexerBase
        where TToken : class, new()
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
            //auto kw = keyWords.find(tok.Text());
            //auto keyword = kw != keyWords.end();
            //if (keyword)
            //    tok.type = kw->second;

            //return tok;
            return new TToken();
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
            //char buff0[2000];
            //va_list ap;
            //va_start(ap, fmt);
            //#ifdef WIN32
            //vsprintf_s(buff0, fmt, ap);
            //#else
            //vsprintf(buff0, fmt, ap);
            //#endif

            //const char *fmt1 = "%s(%d):[%d]: %s\n";
            //char buff[2000];
            //#ifdef WIN32
            //sprintf_s(buff, fmt1, "", tok.lineNumber, tok.slice.Start, buff0);
            //#else
            //sprintf(buff, fmt1, "", tok.lineNumber, tok.slice.Start, buff0);
            //#endif
            //int beforeContext = 2;
            //int afterContext = 2;

            //const LexerBase &lex = *tok.lexer;
            //int start = std::max(0, tok.lineNumber - beforeContext);
            //int end = std::min((int)lex.GetLines().size() - 1, tok.lineNumber + afterContext);

            //std::stringstream err;
            //err << buff << std::endl;
            //for (int n = start; n <= end; ++n)
            //{
            //    for (auto ch : lex.GetLine(n))
            //    {
            //        if (ch == '\t')
            //            err << "    ";
            //        else
            //            err << ch;
            //    }

            //    if (n == tok.lineNumber)
            //    {
            //        for (int ch = 0; ch < (int)lex.GetLine(n).size(); ++ch)
            //        {
            //            if (ch == tok.slice.Start)
            //            {
            //                err << '^';
            //                break;
            //            }

            //            auto c = lex.GetLine(tok.lineNumber)[ch];
            //            if (c == '\t')
            //                err << "    ";
            //            else
            //                err << ' ';
            //        }

            //        err << std::endl;
            //    }
            //}

            //return err.str();
            return "";
        }

        protected List<TToken> _tokens = new List<TToken>();
        protected Dictionary<string, TEnum> _keyWords;
        protected TTokenFactory _factory;
    }
}

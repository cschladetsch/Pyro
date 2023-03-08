// (C) 2023 christian.schladetsch@gmail.com

namespace Pyro.Language.Tau.Lexer {
    using Impl;

    public class TauLexer
        : LexerCommon<ETauToken, TauToken, TauTokenFactory> {
        public TauLexer(string input)
            : base(input) {
        }

        protected override void AddKeyWords() {
            _keyWords.Add("namespace", ETauToken.Namespace);
            _keyWords.Add("interface", ETauToken.Interface);
            _keyWords.Add("event", ETauToken.Event);
            _keyWords.Add("void", ETauToken.Void);
            _keyWords.Add("int", ETauToken.Int);
            _keyWords.Add("string", ETauToken.String);
            _keyWords.Add("float", ETauToken.Float);
            _keyWords.Add("get", ETauToken.Getter);
            _keyWords.Add("set", ETauToken.Setter);
            _keyWords.Add("Func", ETauToken.Func);
            _keyWords.Add("List", ETauToken.List);
            _keyWords.Add("Set", ETauToken.Set);
            _keyWords.Add("Map", ETauToken.Dict);
        }

        protected override bool NextToken() {
            var current = Current();
            if (current == 0)
                return false;

            if (char.IsLetter(current) || current == '_')
                return IdentOrKeyword();

            if (char.IsDigit(current)) {
                var start = Gather(char.IsDigit);
                if (Current() == '.') {
                    Next();
                    var end = Gather(char.IsDigit);
                    if (start.LineNumber != end.LineNumber)
                        return Fail("Bad float literal");

                    return AddSlice(ETauToken.Float, new Slice(this, start.Start, end.End));
                }

                return AddSlice(ETauToken.Int, start);
            }

            switch (current) {
                // TODO: This means we can't use ` at all in embedded Pi code.
                case '(': return Add(ETauToken.OpenParan);
                case ')': return Add(ETauToken.CloseParan);
                case ' ': return AddSlice(ETauToken.WhiteSpace, Gather(c => c == ' '));
                case '\t': return AddSlice(ETauToken.WhiteSpace, Gather(c => c == ' ' || c == '\t')); 
                case ',': return Add(ETauToken.Comma);
                case ';': return Add(ETauToken.Semi);
                case '{': return Add(ETauToken.OpenBrace);
                case '}': return Add(ETauToken.CloseBrace);
                case '<': return Add(ETauToken.LessThan);
                case '>': return Add(ETauToken.GreaterThan);
                case '\r': {
                        // fuck I hate this
                        Next();
                        return true;
                    }
                case '\n': return Add(ETauToken.NewLine);
                case '/':
                    if (Peek() == '/') {
                        Next();
                        var start = _offset + 1;
                        while (Next() != '\n')
                            /* skip comment */
                            ;

                        var comment = _Factory.NewToken(
                            ETauToken.Comment,
                            new Slice(this, start, _offset));
                        _Tokens.Add(comment);
                        return true;
                    }
                    break;
            }

            LexError($"Unrecognised TauToken '{current}'.");

            return false;
        } 
        
        protected override void AddKeywordOrIdent(Slice slice) {
            _Tokens.Add(_keyWords.TryGetValue(slice.Text, out var tok)
                ? _Factory.NewToken(tok, slice)
                : _Factory.NewTokenIdent(slice));
        }

        protected override void Terminate()
            => _Tokens.Add(_Factory.NewToken(ETauToken.Nop, new Slice(this, _offset, _offset)));
    }
}

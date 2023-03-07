﻿namespace Pyro.RhoLang.Lexer {
    using Language;
    using Pyro.Language.Impl;
    using System.Diagnostics.CodeAnalysis;

    /// <inheritdoc />
    /// <summary>
    /// Lexer for the Rho language
    /// </summary>
    public class RhoLexer
        : LexerCommon<ERhoToken, RhoToken, RhoTokenFactory> {
        public RhoLexer(string input)
            : base(input) {
        }

        protected override void AddKeyWords() {
            _keyWords.Add("break", ERhoToken.Break);
            _keyWords.Add("if", ERhoToken.If);
            _keyWords.Add("else", ERhoToken.Else);
            _keyWords.Add("for", ERhoToken.For);
            _keyWords.Add("true", ERhoToken.True);
            _keyWords.Add("false", ERhoToken.False);
            _keyWords.Add("yield", ERhoToken.Yield);
            _keyWords.Add("fun", ERhoToken.Fun);
            _keyWords.Add("return", ERhoToken.Return);
            _keyWords.Add("self", ERhoToken.Self);
            _keyWords.Add("while", ERhoToken.While);
            _keyWords.Add("assert", ERhoToken.Assert);
            _keyWords.Add("writeln", ERhoToken.WriteLine);
            _keyWords.Add("write", ERhoToken.Write);
            _keyWords.Add("new", ERhoToken.New);
            _keyWords.Add("in", ERhoToken.In);
            _keyWords.Add("class", ERhoToken.Class);
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

                    return AddSlice(ERhoToken.Float, new Slice(this, start.Start, end.End));
                }

                return AddSlice(ERhoToken.Int, start);
            }

            switch (current) {
                case Pathname.Quote: return Add(ERhoToken.Quote);
                // TODO: This means we can't use ` at all in embedded Pi code.
                case '`': return AddEmbeddedPi();
                case '{': return Add(ERhoToken.OpenBrace);
                case '}': return Add(ERhoToken.CloseBrace);
                case '(': return Add(ERhoToken.OpenParan);
                case ')': return Add(ERhoToken.CloseParan);
                case ':': return Add(ERhoToken.Colon);
                case ' ': return AddSlice(ERhoToken.Space, Gather(c => c == ' '));
                case ',': return Add(ERhoToken.Comma);
                case '*': return Add(ERhoToken.Multiply);
                case '[': return Add(ERhoToken.OpenSquareBracket);
                case ']': return Add(ERhoToken.CloseSquareBracket);
                case '=': return AddIfNext('=', ERhoToken.Equiv, ERhoToken.Assign);
                case '!': return AddIfNext('=', ERhoToken.NotEquiv, ERhoToken.Not);
                case '&': return AddIfNext('&', ERhoToken.And, ERhoToken.BitAnd);
                case '|': return AddIfNext('|', ERhoToken.Or, ERhoToken.BitOr);
                case '<': return AddIfNext('=', ERhoToken.LessEquiv, ERhoToken.Less);
                case '>': return AddIfNext('=', ERhoToken.GreaterEquiv, ERhoToken.Greater);
                case '"': return LexString();
                case '^': return Add(ERhoToken.Xor);
                case ';': return Add(ERhoToken.Semi);
                case '\t': return Add(ERhoToken.Tab);
                case '\r': {
                        // fuck I hate this
                        Next();
                        return true;
                    }
                case '\n': return Add(ERhoToken.NewLine);
                case '-':
                    if (char.IsDigit(Peek()))
                        return AddSlice(ERhoToken.Int, Gather(char.IsDigit));
                    if (Peek() == '-')
                        return AddTwoCharOp(ERhoToken.Decrement);
                    if (Peek() == '=')
                        return AddTwoCharOp(ERhoToken.MinusAssign);
                    return Add(ERhoToken.Minus);

                case '.':
                    if (Peek() == '.') {
                        Next();
                        if (Peek() == '.') {
                            Next();
                            return Add(ERhoToken.Resume, 3);
                        }
                        return Fail("Two dots doesn't work.");
                    }
                    return Add(ERhoToken.Dot);

                case '+':
                    if (Peek() == '+')
                        return AddTwoCharOp(ERhoToken.Increment);
                    if (Peek() == '=')
                        return AddTwoCharOp(ERhoToken.PlusAssign);
                    return Add(ERhoToken.Plus);

                case '/':
                    if (Peek() == '/') {
                        Next();
                        var start = _offset + 1;
                        while (Next() != '\n')
                            /* skip comment */
                            ;

                        var comment = _Factory.NewToken(
                            ERhoToken.Comment,
                            new Slice(this, start, _offset));
                        _Tokens.Add(comment);
                        return true;
                    }

                    //return LexError("/ is not a valid RhoToken");//Add(ERhoToken.Divide);
                    return Add(ERhoToken.Separator);
            }

            LexError($"Unrecognised RhoToken '{current}'.");

            return false;
        }

        private bool AddEmbeddedPi() {
            Next();
            AddSlice(ERhoToken.PiSlice, Gather(c => c != '`'));
            if (Current() != '`')
                return Fail("Unterminated embedded Pi code");
            Next();
            return true;
        }

        protected override void AddKeywordOrIdent(Slice slice) {
            _Tokens.Add(_keyWords.TryGetValue(slice.Text, out var tok)
                ? _Factory.NewToken(tok, slice)
                : _Factory.NewTokenIdent(slice));
        }

        protected override void Terminate()
            => _Tokens.Add(_Factory.NewToken(ERhoToken.Nop, new Slice(this, _offset, _offset)));
    }
}


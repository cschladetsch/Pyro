using System.Runtime.InteropServices;

namespace Pyro.RhoLang.Parser
{
    using Language;
    using Language.Impl;
    using Lexer;

    /// <inheritdoc cref="IParser" />
    /// <summary>
    /// Parser for the in-fix Rho language that uses tabs for block definitions like Python.
    /// </summary>
    public partial class RhoParser
        : ParserCommon<RhoLexer, RhoAstNode, RhoToken, ERhoToken, ERhoAst, RhoAstFactory>
        , IParser
    {
        public RhoAstNode Result => _Stack.Peek();

        private readonly EStructure _structure;

        public RhoParser(RhoLexer lexer, IRegistry reg, EStructure st)
            : base(lexer, reg)
        {
            _Current = 0;
            _structure = st;
        }

        public bool Process()
        {
            if (_Lexer.Failed)
                return Fail(_Lexer.Error);

            RemoveWhitespace();

            return Parse(_structure);
        }

        private bool Parse(EStructure st)
        {
            if (st != EStructure.Expression)
                _Stack.Push(NewNode(ERhoAst.Program));

            var result = false;
            switch (st)
            {
                case EStructure.Program:
                    result = Program();
                    break;

                case EStructure.Statement:
                    result = Statement();
                    break;

                case EStructure.Expression:
                    result = Expression();
                    break;
            }

            if (Failed || !result)
                return false;

            ConsumeNewLines();

            if (!Try(ERhoToken.Nop))
                return FailLocation("Unexpected extra stuff found");

            return _Stack.Count == 1 || InternalFail("Semantic stack not empty after parsing");
        }

        private bool Program()
        {
            while (!Failed && !Try(ERhoToken.Nop))
                if (!Statement())
                {
                    var c = Current();
                    return false;
                }

            return true;
        }

        private bool Block()
        {
            ConsumeNewLines();

            var indent = 0;
            while (TryConsume(ERhoToken.Tab))
                ++indent;

            if (indent == 0)
                return false;

            Push(NewNode(ERhoAst.Block));
            while (!Failed)
            {
                if (Try(ERhoToken.Pass))
                    return true;

                if (!Statement())
                    return FailLocation("Statement expected");

                ConsumeNewLines();

                var level = 0;
                while (TryConsume(ERhoToken.Tab))
                    ++level;

                if (level == indent - 1 || level == indent + 1)
                {
                    // return to start so top block can continue
                    //_Current -= indent;
                    return true;
                }

                if (level != indent)
                    return FailLocation("Mismatch block indent");
            }

            return false;
        }

        private bool TryConsume(ERhoToken token)
        {
            ConsumeNewLines();
            if (!Try(token))
                return false;

            Consume();
            return true;
        }

        private void RemoveWhitespace()
        {
            var prevNewLine = true;
            foreach (var tok in _Lexer.Tokens)
            {
                // remove useless consecutive newlines
                var newLine = tok.Type == ERhoToken.NewLine;
                if (prevNewLine && newLine)
                    continue;

                prevNewLine = newLine;

                switch (tok.Type)
                {
                    // keep tabs!
                    case ERhoToken.Space:
                    case ERhoToken.Comment:
                        continue;
                }

                _Tokens.Add(tok);
            }
        }

        private void ConsumeNewLines()
        {
            while (Try(ERhoToken.NewLine))
                Consume();
        }
    }
}


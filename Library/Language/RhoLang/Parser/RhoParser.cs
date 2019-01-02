using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Remoting.Activation;

namespace Diver.Language
{
    /// <summary>
    /// Parser for the in-fix Rho language that uses tabs for block definitions like Python.
    /// </summary>
    public partial class RhoParser
        : ParserCommon<RhoLexer, RhoAstNode, RhoToken, ERhoToken, ERhoAst, RhoAstFactory>
    {
        public RhoParser(RhoLexer lexer, IRegistry reg, EStructure st)
            : base(lexer, reg)
        {
            _current = 0;
            _indent = 0;
            _structure = st;
        }

        public RhoAstNode Result => _stack.Peek();

        public bool Process()
        {
            if (_lexer.Failed)
                return Fail(_lexer.Error);

            RemoveWhitespace();

            return Parse(_structure);
        }

        private bool Parse(EStructure st)
        {
            _stack.Push(NewNode(ERhoAst.Program));

            bool result = false;
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

            if (!Try(ERhoToken.None))
                return Fail("Unexpected extra stuff found");

            return _stack.Count == 1 || Fail("Stack not empty after parsing");
        }

        private bool Program()
        {
            while (!Failed && !Try(ERhoToken.None))
            {
                if (!Statement())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// NOTE: General idea is to make functions like assignments:
        ///
        /// This will hopefully solve the nested function problem that's been
        /// kicking my arse for two nights:
        ///
        /// fun foo()
        ///     fun bar()
        ///         assert(true)
        ///     bar()
        /// foo()
        ///
        /// { { true assert } 'bar # } bar & } 'foo # foo &
        ///                            ^^^ unresolved!
        ///
        /// The above results in "unresolved symbol 'bar'", because the inner bar
        /// function was stored into a local scope and not stored in 'foo's scope.
        ///
        /// No amount of fuckery in the Translator or Executor could fix this problem.
        /// I was wrong-thinking from the start, because of the very way I implemented
        /// parsing functions in the parser.
        ///
        /// The correct way seems to be to make new functions as normal assignments:
        /// 1 'a #      // normal assignment
        /// {} 'b #     // function assignment- note it's not wrapped in its own scope!
        ///
        /// Before, this would become:
        /// { {} 'b # }     // no good! b can't be seen outside it's own scope!
        ///
        /// This would result in something like for the above:
        /// {true assert} 'bar # bar & 'foo # foo &
        /// 
        /// Where the new continuation is just added directly to local scope, and not its
        /// own `hidden` scope`.
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool Function()
        {
            var cont = NewNode(Consume());
            var ident = QuotedIdent();

            Expect(ERhoToken.OpenParan);
            var args = NewNode(ERhoAst.ArgList);

            if (Try(ERhoToken.Ident))
            {
                args.Add(Consume());
                while (Try(ERhoToken.Comma))
                {
                    Consume();
                    args.Add(Expect(ERhoToken.Ident));
                }
            }

            Expect(ERhoToken.CloseParan);
            Expect(ERhoToken.NewLine);

            if (Failed)
                return false;

            if (!Block())
                return CreateError("Block expected");
            var block = Pop();
            cont.Add(ident);
            cont.Add(args);
            cont.Add(block);

            // making a new function is just an assignment from a continuation
            var assign = NewNode(ERhoAst.Assignment);
            assign.Add(cont);
            assign.Add(ident);

            Append(assign);

            PrintTree();

            return !Failed;
        }

        private bool While()
        {
            var wile = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return CreateError("While what?");
            Expect(ERhoToken.CloseParan);
            wile.Add(Pop());

            if (!Block())
                return CreateError("No While body");
            wile.Add(Pop());
            Append(wile);
            return true;
        }

        private bool Block()
        {
            ConsumeNewLines();

            if (!Try(ERhoToken.Tab))
                return false;

            Push(NewNode(ERhoAst.Block));
            var indent = 1;
            while (!Failed)
            {
                ConsumeNewLines();

                var level = 0;
                while (TryConsume(ERhoToken.Tab))
                    ++level;

                if (level < indent)
                    return true;

                if (level != indent)
                    return CreateError("Mismatch block indent");

                if (Try(ERhoToken.None))
                    return true;

                if (!Statement())
                    return FailWith("Statement expected");
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

        private bool CreateError(string text, params object[] args)
        {
            return Fail(_lexer.CreateErrorMessage(Current(), string.Format(text, args)));
        }

        private void RemoveWhitespace()
        {
            var prevNl = true;
            foreach (var tok in _lexer.Tokens)
            {
                // remove useless consecutive newlines
                var nl = tok.Type == ERhoToken.NewLine;
                if (prevNl && nl)
                    continue;

                prevNl = nl;

                switch (tok.Type)
                {
                    // keep tabs!
                    case ERhoToken.Space:
                    case ERhoToken.Comment:
                        continue;
                }

                _tokens.Add(tok);
            }
        }

        private void ConsumeNewLines()
        {
            while (Try(ERhoToken.NewLine))
                Consume();
        }

        protected RhoAstNode QuotedIdent()
        {
            return new RhoAstNode(ERhoAst.Ident, new Label(Expect(ERhoToken.Ident).Text, true));
        }

        private readonly EStructure _structure;
    }
}

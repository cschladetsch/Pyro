using System;
using System.Linq;

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
                if (Statement())
                    continue;
                return false;
            }

            return true;
        }

        private RhoAstNode QuotedIdent()
        {
            return new RhoAstNode(ERhoAst.Ident, new Label(Expect(ERhoToken.Ident).Text, true));
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
            var cont = PushConsume();
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
            assign.Add(assign);

            Append(Pop());

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
            Push(wile);
            return true;
        }

        private bool Block()
        {
            ConsumeNewLines();

            Push(NewNode(ERhoAst.Block));
            ++indent;
            while (!Failed)
            {
                int level = 0;
                while (Try(ERhoToken.Tab))
                {
                    ++level;
                    Consume();
                }

                if (Try(ERhoToken.NewLine))
                {
                    Consume();
                    continue;
                }

                // close current block
                if (level < indent)
                {
                    --indent;

                    // rewind to start of tab sequence to determine next block
                    --current;
                    while (Try(ERhoToken.Tab))
                        --current;

                    ++current;

                    return true;
                }

                if (level != indent)
                {
                    return CreateError("Mismatch block indent");
                }

                if (Statement())
                {
                    Append(Pop());
                }
            }

            return false;
        }

        private void ConsumeNewLines()
        {
            while (Try(ERhoToken.NewLine))
                Consume();
        }


        private bool Statement()
        {
            var type = Current().Type;

            switch (type)
            {
                case ERhoToken.WriteLine:
                case ERhoToken.Write:
                {
                    return Write();
                }

                case ERhoToken.Assert:
                {
                    return Assert();
                }

                case ERhoToken.Return:
                case ERhoToken.Yield:
                {
                    var ret = NewNode(Consume());
                    if (Expression())
                        ret.Add(Pop());
                    Append(ret);
                    goto finis;
                }

                case ERhoToken.While:
                {
                    return While();
                }

                case ERhoToken.For:
                {
                    //return For();
                    throw new NotImplementedException("For statements");
                }

                case ERhoToken.If:
                {
                    return IfCondition();
                }

                case ERhoToken.Fun:
                {
                    return Function();
                }
            }

            if (Try(ERhoToken.None))
                return true;

            if (!Expression())
                return false;

            finis:

            if (!Try(ERhoToken.None))
                Expect(ERhoToken.NewLine);

            return true;
        }

        private bool Assert()
        {
            var assert = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailWith("Assert needs an expression to test");
            Expect(ERhoToken.CloseParan);
            assert.Add(Pop());
            Append(assert);
            return true;
        }

        private bool Write()
        {
            var write = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailWith("Write what?");
            Expect(ERhoToken.CloseParan);
            write.Add(Pop());
            Append(write);
            return false;
        }

        private bool Call()
        {
            // eat the opening paranthesis
            Consume();

            var call = NewNode(ERhoAst.Call);
            var args = NewNode(ERhoAst.ArgList);

            call.Add(Pop());        // the thing to call
            call.Add(args);
            Push(call);

            if (Expression())
            {
                args.Add(Pop());
                while (Try(ERhoToken.Comma))
                {
                    Consume();
                    if (!Expression())
                    {
                        return CreateError("What is the next argument?");
                    }

                    args.Add(Pop());
                }
            }

            Expect(ERhoToken.CloseParan);
            Append(Pop());
            return true;
        }

        private bool GetMember()
        {
            PushConsume();
            Append(Expect(ERhoToken.Ident));
            return true;
        }

        private bool IfCondition()
        {
            // make the conditional node in AST
            var cond = NewNode(Consume());

            if (!Expression())
                return CreateError("If what?");
            var condition = Pop();

            // get the true-clause
            if (!Block())
                return CreateError("If needs a block");
            cond.Add(condition);
            cond.Add(Pop());

            // if there's an else, add it as well
            if (Try(ERhoToken.Else))
            {
                Consume();
                if (!Block())
                    return CreateError("No else block");
                cond.Add(Pop());
            }

            Append(cond);
            return true;
        }

        private bool IndexOp()
        {
            PushConsume();
            if (!Expression())
                return CreateError("Index what?");

            Expect(ERhoToken.CloseSquareBracket);
            Append(Pop());
            return true;
        }

        private void For(RhoAstNode block)
        {
            if (!Try(ERhoToken.For))
                return;

            Consume();

            var f = NewNode(ERhoAst.For);
            if (!Expression())
            {
                CreateError("For what?");
                return;
            }

            if (Try(ERhoToken.In))
            {
                Consume();
                f.Add(Pop());

                if (!Expression())
                {
                    CreateError("For each in what?");
                    return;
                }

                f.Add(Pop());
            }
            else
            {
                Expect(ERhoToken.Semi);
                f.Add(Pop());

                if (!Expression())
                {
                    CreateError("When does the for statement stop?");
                    return;
                }

                f.Add(Pop());
                Expect(ERhoToken.Semi);

                if (!Expression())
                {
                    CreateError("What happens when a for statement ends?");
                    return;
                }

                f.Add(Pop());
            }

            Expect(ERhoToken.NewLine);
            //AddBlock(f);
            throw new NotImplementedException();
            block.Add(f);
        }

        private bool CreateError(string text, params object[] args)
        {
            return Fail(_lexer.CreateErrorMessage(Current(), string.Format(text, args)));
        }

        private int indent;
        private int current;
        private readonly EStructure _structure;
    }
}

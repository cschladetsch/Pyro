using System;

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

        public bool Process()
        {
            if (_lexer.Failed)
                return Fail(_lexer.Error);

            RemoveWhitespace();

            _root = NewNode(ERhoAst.Program);

            return Run(_structure);
        }

        private void RemoveWhitespace()
        {
            foreach (var tok in _lexer.Tokens)
            {
                switch (tok.Type)
                {
                    // keep tabs!
                    case ERhoToken.Whitespace:
                    case ERhoToken.Comment:
                        continue;
                }

                _tokens.Add(tok);
            }
        }

        private bool Run(EStructure st)
        {
            switch (st)
            {
                case EStructure.Statement:
                    if (!Statement(_root))
                        return CreateError("Statement expected");
                    break;

                case EStructure.Expression:
                    if (!Expression())
                        return CreateError("Expression expected");
                    ConsumeNewLines();
                    if (!Try(ERhoToken.None))
                        return Fail("Unexpected extra stuff found");
                    _root.Add(Pop());
                    break;

                case EStructure.Function:
                    return Function(_root);

                case EStructure.Program:
                    return Program();
            }

            ConsumeNewLines();
            return _stack.Count == 0 || Fail("Stack not empty after parsing");
        }

        private bool Program()
        {
            while (!Try(ERhoToken.None) && !Failed)
            {
                ConsumeNewLines();
                if (Statement(_root))
                    continue;
                return CreateError("Statement expected");
            }
            return true;
        }

        RhoAstNode QuotedIdent()
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
        private bool Function(RhoAstNode node)
        {
            ConsumeNewLines();

            Expect(ERhoToken.Fun);
            var cont = NewNode(ERhoAst.Function);
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

            var code = NewNode(ERhoAst.Block);
            Block(code);
            cont.Add(ident);
            cont.Add(args);
            cont.Add(code);
            var assign = NewNode(ERhoAst.Assignment);
            assign.Add(cont);
            assign.Add(Expect(ERhoToken.Ident));
            node.Add(assign);

            return !Failed;
        }

        private void While(RhoAstNode block)
        {
            var w = NewNode(Consume());
            if (!Expression())
            {
                CreateError("While what?");
                return;
            }

            w.Add(Pop());
            block.Add(w);
        }

        /// <summary>
        /// This seems to be broken too
        /// </summary>
        /// <param name="node"></param>
        private void Block(RhoAstNode node)
        {
            ConsumeNewLines();

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

                    return;
                }

                if (level != indent)
                {
                    CreateError("Mismatch block indent");
                    return;
                }

                Statement(node);
            }
        }

        private bool Statement(RhoAstNode block)
        {
            ConsumeNewLines();
            var type = Current().Type;

            switch (type)
            {
                case ERhoToken.WriteLine:
                case ERhoToken.Write:
                {
                    var write = NewNode(Consume());
                    Expect(ERhoToken.OpenParan);
                    if (!Expression())
                        return FailWith("Write what?");
                    Expect(ERhoToken.CloseParan);
                    write.Add(Pop());
                    block.Add(write);
                    goto finis;
                }

                case ERhoToken.Assert:
                {
                    var assert = NewNode(Consume());
                    Expect(ERhoToken.OpenParan);
                    if (!Expression())
                    {
                        Fail(_lexer.CreateErrorMessage(Current(), "Assert needs an expression to test"));
                        return false;
                    }
                    Expect(ERhoToken.CloseParan);
                    assert.Add(Pop());
                    block.Add(assert);
                    goto finis;
                }

                case ERhoToken.Return:
                case ERhoToken.Yield:
                {
                    var ret = NewNode(Consume());
                    if (Expression())
                        ret.Add(Pop());
                    block.Add(ret);
                    goto finis;
                }

                case ERhoToken.While:
                {
                    While(block);
                    return true;
                }

                case ERhoToken.For:
                {
                    For(block);
                    return true;
                }

                case ERhoToken.If:
                {
                    IfCondition(block);
                    return true;
                }

                case ERhoToken.Fun:
                {
                    return Function(block);
                }
            }

            ConsumeNewLines();

            if (Try(ERhoToken.None))
                return true;

            if (!Expression())
                return false;

            block.Add(Pop());

            finis:

            //// statements can end with an optional semi followed by a new line
            //if (Try(ERhoToken.Semi))
            //    Consume();

            if (!Try(ERhoToken.None))
                Expect(ERhoToken.NewLine);

            return true;
        }

        private void Call()
        {
            Consume();

            var call = NewNode(ERhoAst.Call);
            var args = NewNode(ERhoAst.ArgList);
            call.Add(Pop());
            call.Add(args);

            if (Expression())
            {
                args.Add(Pop());
                while (Try(ERhoToken.Comma))
                {
                    Consume();
                    if (!Expression())
                    {
                        CreateError("What is the next argument?");
                        return;
                    }

                    args.Add(Pop());
                }
            }

            Push(call);
            Expect(ERhoToken.CloseParan);

            if (Try(ERhoToken.Replace))
                call.Add(Consume());
        }

        private void GetMember()
        {
            Consume();
            var get = NewNode(ERhoAst.GetMember);
            get.Add(Pop());
            get.Add(Expect(ERhoToken.Ident));
            Push(get);
        }

        private void IfCondition(RhoAstNode block)
        {
            if (!Try(ERhoToken.If))
                return;

            Consume();

            if (!Expression())
            {
                CreateError("If what?");
                return;
            }

            var condition = Pop();

            // get the true-clause
            var trueClause = NewNode(ERhoAst.Block);
            Block(trueClause);

            // make the conditional node in AST
            var cond = NewNode(ERhoAst.Conditional);
            cond.Add(condition);
            cond.Add(trueClause);

            // if there's an else, add it as well
            if (Try(ERhoToken.Else))
            {
                Consume();
                var falseClause = NewNode(ERhoAst.Block);
                Block(falseClause);
                cond.Add(falseClause);
            }

            block.Add(cond);
        }

        private void IndexOp()
        {
            Consume();
            var index = NewNode(ERhoAst.IndexOp);
            index.Add(Pop());
            if (!Expression())
            {
                CreateError("Index what?");
                return;
            }

            Expect(ERhoToken.CloseSquareBracket);
            index.Add(Pop());
            Push(index);
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

        private void ConsumeNewLines()
        {
            while (Try(ERhoToken.NewLine))
                Consume();
        }

        private int indent;
        private int current;
        private EStructure _structure;
    }
}

namespace Diver.Language
{
    /// <summary>
    /// Parser for the in-fix Rho language that uses tabs for block definitions like Python.
    /// </summary>
    public class RhoParser
        : ParserCommon<RhoLexer, RhoAstNode, RhoToken, ERhoToken, ERhoAst, RhoAstFactory>
    {
        public RhoParser(LexerBase lexer)
            : base(lexer, null)
        {
        }

        public override bool Process(RhoLexer lex, EStructure structure)
        {
            _current = 0;
            _indent = 0;
            _lexer = lex;

            if (_lexer.Failed)
                return Fail(_lexer.Error);

            RemoveWhitespace();

            _root = NewNode(ERhoAst.Program);

            return Run(structure);
        }

        private void RemoveWhitespace()
        {
            foreach (var tok in _lexer.Tokens)
            {
                switch (tok.Type)
                {
                    case ERhoToken.Whitespace:
                    case ERhoToken.Comment:
                        continue;
                }

                _tokens.Add(tok);
            }
        }

        private new bool Run(EStructure st)
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
                return Fail("Statement expected");
            }
            return true;
        }

        private bool Function(RhoAstNode node)
        {
            ConsumeNewLines();

            Expect(ERhoToken.Fun);
            var name = Expect(ERhoToken.Ident);
            var fun = NewNode(ERhoAst.Function);
            fun.Add(new RhoAstNode(ERhoAst.Ident, new Label(name.Text, true)));
            Expect(ERhoToken.OpenParan);
            var args = NewNode(ERhoAst.ArgList);
            fun.Add(args);

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

            AddBlock(fun);
            node.Add(fun);

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

        private void AddBlock(RhoAstNode fun)
        {
            var block = NewNode(ERhoAst.Block);
            Block(block);
            fun.Add(block);
        }

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
            switch (Current().Type)
            {
                case ERhoToken.Assert:
                {
                    var ass = NewNode(Consume());
                    Expect(ERhoToken.OpenParan);
                    if (!Expression())
                    {
                        Fail(_lexer.CreateErrorMessage(Current(), "Assert needs an expression to test"));
                        return false;
                    }
                    Expect(ERhoToken.CloseParan);
                    ass.Add(Pop());
                    block.Add(ass);
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
                    Function(block);
                    return true;
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

        private bool Expression()
        {
            if (!Logical())
                return false;

            if (Try(ERhoToken.Assign) 
                || Try(ERhoToken.PlusAssign) 
                || Try(ERhoToken.MinusAssign) 
                //|| Try(ERhoToken.MulAssign) 
                //|| Try(ERhoToken.DivAssign)
                )
            {
                var node = NewNode(Consume());
                var ident = Pop();
                if (!Logical())
                {
                    CreateError("Assignment requires an expression");
                    return false;
                }

                node.Add(Pop());
                node.Add(ident);
                Push(node);
            }

            return true;
        }

        private bool Logical()
        {
            if (!Relational())
                return false;

            while (Try(ERhoToken.And) || Try(ERhoToken.Or) || Try(ERhoToken.Xor))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Relational())
                    return CreateError("Relational expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        bool Relational()
        {
            if (!Additive())
                return false;

            while (Try(ERhoToken.Less) || Try(ERhoToken.Greater) || Try(ERhoToken.Equiv) || Try(ERhoToken.NotEquiv)
                || Try(ERhoToken.LessEquiv) || Try(ERhoToken.GreaterEquiv))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Additive())
                    return CreateError("Additive expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        bool Additive()
        {
            // unary +/- operator
            if (Try(ERhoToken.Plus) || Try(ERhoToken.Minus))
            {
                var uniSigned = NewNode(Consume());
                if (!Term())
                    return CreateError("Term expected");

                uniSigned.Add(Pop());
                Push(uniSigned);
                return true;
            }

            if (Try(ERhoToken.Not))
            {
                var negate = NewNode(Consume());
                if (!Additive())
                    return CreateError("Additive expected");

                negate.Add(Pop());
                Push(negate);
                return true;
            }

            if (!Term())
                return false;

            while (Try(ERhoToken.Plus) || Try(ERhoToken.Minus))
            {
                //[1]
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Term())
                    return CreateError("Term expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Term()
        {
            if (!Factor())
                return false;

            while (Try(ERhoToken.Multiply) || Try(ERhoToken.Divide))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Factor())
                    return CreateError("Factor expected with a term");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Factor()
        {
            if (Try(ERhoToken.OpenParan))
            {
                var exp = NewNode(Consume());
                if (!Expression())
                    return CreateError("Expected an expression");

                Expect(ERhoToken.CloseParan);
                exp.Add(Pop());
                Push(exp);
                return true;
            }

            if (Try(ERhoToken.OpenSquareBracket))
            {
                var list = NewNode(ERhoAst.List);
                do
                {
                    Consume();
                    if (Try(ERhoToken.CloseSquareBracket))
                        break;
                    if (Expression())
                        list.Add(Pop());
                    else
                    {
                        Fail("Badly formed array");
                        return false;
                    }
                }
                while (Try(ERhoToken.Comma));

                Expect(ERhoToken.CloseSquareBracket);
                Push(list);

                return true;
            }

            if (   Try(ERhoToken.Int) 
                || Try(ERhoToken.Float) 
                || Try(ERhoToken.String) 
                || Try(ERhoToken.True) 
                || Try(ERhoToken.False)
                )
            {
                return PushConsume();
            }

            if (Try(ERhoToken.Self))
                return PushConsume();

            //    while (Try(ERhoToken.Lookup))
            //        return PushConsume();

            if (Try(ERhoToken.Ident))
                return ParseFactorIdent();

            if (Try(ERhoToken.Pathname))
                return ParseFactorIdent();

            return false;
        }

        private bool ParseFactorIdent()
        {
            PushConsume();

            while (!Failed)
            {
                if (Try(ERhoToken.Dot))
                {
                    ParseGetMember();
                    continue;
                }

                if (Try(ERhoToken.OpenParan))
                {
                    ParseMethodCall();
                    continue;
                }

                if (Try(ERhoToken.OpenSquareBracket))
                {
                    ParseIndexOp();
                    continue;
                }

                break;
            }

            return true;
        }

        private void ParseMethodCall()
        {
            Consume();
            var call = NewNode(ERhoAst.Call);
            call.Add(Pop());
            var args = NewNode(ERhoAst.ArgList);
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

        private void ParseGetMember()
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

        private void ParseIndexOp()
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
            AddBlock(f);
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
    }
}

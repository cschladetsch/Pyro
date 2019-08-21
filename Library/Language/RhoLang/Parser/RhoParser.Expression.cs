namespace Pyro.RhoLang.Parser
{
    using Language.Lexer;
    using Language.Parser;
    using Lexer;

    /// <summary>
    /// Functions that deal only with parsing expressions.
    ///
    /// NOTE in Rho, a statement can also be an expression.
    /// </summary>
    public partial class RhoParser
    {
        private bool Expression()
        {
            if (!Logical())
                return false;

            if (   Try(ERhoToken.Assign)
                || Try(ERhoToken.PlusAssign)
                || Try(ERhoToken.MinusAssign)
                || Try(ERhoToken.MulAssign)
                || Try(ERhoToken.DivAssign)
               )
            {
                var assign = NewNode(Consume());
                var ident = Pop();
                if (!Logical())
                    return FailLocation("Logical sub-expression expected");

                assign.Add(Pop());
                assign.Add(ident);
                Push(assign);
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
                    return FailLocation("Relational sub-expression expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Relational()
        {
            if (!Additive())
                return false;

            while (   Try(ERhoToken.Less)
                   || Try(ERhoToken.Greater)
                   || Try(ERhoToken.Equiv)
                   || Try(ERhoToken.NotEquiv)
                   || Try(ERhoToken.LessEquiv)
                   || Try(ERhoToken.GreaterEquiv)
                )
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Additive())
                    return FailLocation("Additive sub-expression expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Additive()
        {
            // unary +/- operator
            if (Try(ERhoToken.Plus) || Try(ERhoToken.Minus))
            {
                var signed = NewNode(Consume());
                if (!Term())
                    return FailLocation("Term expected");

                signed.Add(Pop());
                return Push(signed);
            }

            if (Try(ERhoToken.Not))
            {
                var negate = NewNode(Consume());
                if (!Additive())
                    return FailLocation("Additive sub-component expected");

                negate.Add(Pop());
                return Push(negate);
            }

            if (!Term())
                return false;

            while (Try(ERhoToken.Plus) || Try(ERhoToken.Minus))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Term())
                    return FailLocation("Term expected");

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
                    return FailLocation("Term expected a factor");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Factor()
        {
            if (Try(ERhoToken.New))
                return New();

            if (Try(ERhoToken.OpenParan))
                return Paran();

            if (Try(ERhoToken.PiSlice))
                return Pi();

            if (TryConsume(ERhoToken.OpenBrace))
                return AddList();

            if (Try(ERhoToken.Self))
                return FactorIdent();

            if (Try(ERhoToken.Ident) || Try(ERhoToken.Pathname))
                return FactorIdent();

            if (   Try(ERhoToken.Int)
                || Try(ERhoToken.Float)
                || Try(ERhoToken.String)
                || Try(ERhoToken.True)
                || Try(ERhoToken.False)
                )
            {
                return PushConsumed();
            }

            return false;
        }

        private bool New()
        {
            var @new = NewNode(Consume());
            if (Expression())
                @new.Add(Pop());
            else
                return FailLocation("new what?");

            return Push(@new);
        }

        private bool AddList()
        {
            var list = NewNode(ERhoAst.List);
            while (true)
            {
                if (TryConsume(ERhoToken.CloseBrace))
                    break;

                if (Expression())
                    list.Add(Pop());
                else
                    return FailLocation("Expression required within array");

                //if (!TryConsume(ERhoToken.Comma))
                //    break;
            }

            //Expect(ERhoToken.CloseSquareBracket);
            return Failed
                ? FailLocation("Closing bracket expected for array")
                : Push(list);
        }

        private bool Paran()
        {
            var exp = NewNode(Consume());
            if (!Expression())
                return FailLocation("Expected an expression");

            Expect(ERhoToken.CloseParan);

            //// want to allow `(1 - 2).ToString()`
            //// But this doesn't work because dots are expected only after Members
            //if (TryConsume(ERhoToken.Dot))
            //{
            //    FactorIdent();
            //}

            exp.Add(Pop());
            return Push(exp);
        }

        private bool Pi()
        {
            var input = Current().Text;
            var lexer = new PiLexer(input);
            if (!lexer.Process())
                return FailLocation(lexer.Error);

            var parser = new PiParser(lexer);
            if (!parser.Process(lexer))
                return FailLocation(parser.Error);

            var pi = NewNode(Consume());
            pi.Value = parser.Root;
            Push(pi);
            return true;
        }

        private bool FactorIdent()
        {
            PushConsume();

            while (!Failed)
            {
                if (Try(ERhoToken.Dot))
                {
                    if (!GetMember())
                        return false;
                }
                else if (Try(ERhoToken.OpenParan))
                {
                    if (!Call())
                        return false;
                }
                else if (Try(ERhoToken.OpenSquareBracket))
                {
                    if (!IndexOp())
                        return false;
                }
                else
                {
                    break;
                }
            }

            return !Failed;
        }

        private bool GetMember()
        {
            Consume();

            var get = NewNode(ERhoAst.GetMember);
            get.Add(Pop());
            get.Add(Expect(ERhoToken.Ident));

            return Push(get);
        }

        private bool Call()
        {
            // eat the opening paranthesis
            Consume();

            var call = NewNode(ERhoAst.Call);
            var args = NewNode(ERhoAst.ArgList);

            // the thing to call is on the parse stack. It could be something like
            // `foo().bar.spam[4](a,b,c)`
            call.Add(Pop());
            call.Add(args);
            Push(call);

            if (Expression())
            {
                args.Add(Pop());
                while (Try(ERhoToken.Comma))
                {
                    Consume();
                    if (!Expression())
                        return FailLocation("Argument expected");

                    args.Add(Pop());
                }
            }

            Expect(ERhoToken.CloseParan);
            return true;
        }

        private bool IndexOp()
        {
            Consume();

            var index = NewNode(ERhoAst.IndexOp);
            index.Add(Pop());
            if (!Expression())
                return FailLocation("Indexing expression expected");

            index.Add(Pop());
            Push(index);

            Expect(ERhoToken.CloseSquareBracket);
            return true;
        }
    }
}


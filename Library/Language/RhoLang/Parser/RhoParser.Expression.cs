using Pyro.RhoLang.Lexer;

namespace Pyro.RhoLang.Parser
{
    /// <summary>
    /// Functions that deal only with parsing expressions.
    ///
    /// NOTE that in Rho, a statement can also be an expression.
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
                    return FailLocation("Assignment requires an expression");

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
                    return FailLocation("Relational component expected");

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
                    return FailLocation("Additive component expected");

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
                    return FailLocation("Additive component expected");

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
                    return FailLocation("Factor expected with a term");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        private bool Factor()
        {
            if (Try(ERhoToken.New))
            {
                var @new = NewNode(Consume());
                @new.Add(Pop());
                return Push(@new);
            }

            if (Try(ERhoToken.OpenParan))
            {
                var exp = NewNode(Consume());
                if (!Expression())
                    return FailLocation("Expected an expression");

                Expect(ERhoToken.CloseParan);
                exp.Add(Pop());
                return Push(exp);
            }

            if (TryConsume(ERhoToken.OpenSquareBracket))
            {
                var list = NewNode(ERhoAst.List);
                while (true)
                {
                    if (TryConsume(ERhoToken.CloseSquareBracket))
                        break;
                    if (Expression())
                        list.Add(Pop());
                    else
                        return FailLocation("Expressions required within array");
                    if (!TryConsume(ERhoToken.Comma))
                        break;
                }

                Expect(ERhoToken.CloseSquareBracket);
                return Failed 
                    ? FailLocation("Closing bracked expected for array")
                    : Push(list);
            }

            if (   Try(ERhoToken.Int) 
                || Try(ERhoToken.Float) 
                || Try(ERhoToken.String) 
                || Try(ERhoToken.True) 
                || Try(ERhoToken.False)
                )
            {
                return PushConsumed();
            }

            if (Try(ERhoToken.Self))
                return PushConsumed();

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

            return true;
        }

        private bool GetMember()
        {
            Consume();

            var get = NewNode(ERhoAst.GetMember);
            get.Add(Pop());
            get.Add(Expect(ERhoToken.Ident));
            Push(get);
            return true;
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
                        return FailLocation("What is the next argument?");

                    args.Add(Pop());
                }
            }

            Expect(ERhoToken.CloseParan);
            return true;
        }

        private bool IndexOp()
        {
            var index = PushConsume();
            index.Add(Pop());
            if (!Expression())
                return FailLocation("Index what?");

            index.Add(Pop());

            Expect(ERhoToken.CloseSquareBracket);
            return true;
        }
    }
}


namespace Diver.Language
{
    /// <summary>
    /// Functions that deal with parsing expressions only
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
                var node = NewNode(Consume());
                var ident = Pop();
                if (!Logical())
                    return FailWith("Assignment requires an expression");

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
                    return FailWith("Relational component expected");

                node.Add(Pop());
                Push(node);
            }

            return true;
        }

        bool Relational()
        {
            if (!Additive())
                return false;

            while (Try(ERhoToken.Less) 
                   || Try(ERhoToken.Greater) 
                   || Try(ERhoToken.Equiv) 
                   || Try(ERhoToken.NotEquiv)
                   || Try(ERhoToken.LessEquiv) 
                   || Try(ERhoToken.GreaterEquiv))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Additive())
                    return FailWith("Additive component expected");

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
                    return FailWith("Term expected");

                uniSigned.Add(Pop());
                Push(uniSigned);
                return true;
            }

            if (Try(ERhoToken.Not))
            {
                var negate = NewNode(Consume());
                if (!Additive())
                    return FailWith("Additive component expected");

                negate.Add(Pop());
                Push(negate);
                return true;
            }

            if (!Term())
                return false;

            while (Try(ERhoToken.Plus) || Try(ERhoToken.Minus))
            {
                var node = NewNode(Consume());
                node.Add(Pop());
                if (!Term())
                    return FailWith("Term expected");

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
                    return FailWith("Factor expected with a term");

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
                    return FailWith("Expected an expression");

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
                        return FailWith("What is the next argument?");

                    args.Add(Pop());
                }
            }

            Expect(ERhoToken.CloseParan);
            return true;
        }

        private bool GetMember()
        {
            PushConsume();
            return Append(Expect(ERhoToken.Ident));
        }

        private bool IndexOp()
        {
            PushConsume();
            if (!Expression())
                return FailWith("Index what?");

            Expect(ERhoToken.CloseSquareBracket);
            return true;
        }
    }
}

namespace Diver.Language
{
    /// <summary>
    /// Rho statements are stand-alone components made of sub-expressions.
    ///
    /// NOTE that Statements do not leave anything on the parsing stack:
    /// They either succeed or leave a decent contextual lexical+semantic error message.
    /// </summary>
    public partial class RhoParser
    {
        private bool Statement()
        {
            ConsumeNewLines();

            switch (Current().Type)
            {
                case ERhoToken.WriteLine:
                case ERhoToken.Write:
                    return Write();

                case ERhoToken.Assert:
                    return Assert();

                case ERhoToken.Return:
                case ERhoToken.Yield:
                case ERhoToken.Resume:
                case ERhoToken.Suspend:
                {
                    var ret = NewNode(Consume());
                    if (Expression())
                        ret.Add(Pop());
                    return Append(ret);
                }

                case ERhoToken.While:
                    return While();

                case ERhoToken.For:
                    return For();

                case ERhoToken.If:
                    return If();

                case ERhoToken.Fun:
                    return Function();

                // TODO: Need a 'pass' for empty blocks
                //case ERhoToken.Pass:
                //    Append(Current().Type);
                //    return true;

                case ERhoToken.None:
                    return false;
            }

            if (!Expression())
                return FailWith("Expression expected");

            return Append(Pop()) && !Try(ERhoToken.None);
        }

        private bool Function()
        {
            var cont = NewNode(Consume());
            var ident = MakeQuotedIdent();

            Expect(ERhoToken.OpenParan);
            var args = NewNode(ERhoAst.ArgList);
            if (Try(ERhoToken.Ident))
            {
                args.Add(Consume());
                while (TryConsume(ERhoToken.Comma))
                    args.Add(Expect(ERhoToken.Ident));
            }

            Expect(ERhoToken.CloseParan);
            Expect(ERhoToken.NewLine);

            if (!Block())
                return FailWith("Block expected");

            // make the continuation
            var block = Pop();
            cont.Add(ident);
            cont.Add(args);
            cont.Add(block);

            // assign it within current scope
            var assign = NewNode(ERhoAst.Assignment);
            assign.Add(cont);
            assign.Add(ident);

            return Append(assign);
        }

        private bool While()
        {
            var @while = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailWith("While what?");

            Expect(ERhoToken.CloseParan);
            @while.Add(Pop());

            if (!Block())
                return FailWith("No While body");

            @while.Add(Pop());
            return Append(@while);
        }

        private bool Assert()
        {
            var assert = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailWith("Assert needs an expression to test");
            Expect(ERhoToken.CloseParan);

            assert.Add(Pop());
            return Append(assert);
        }

        private bool Write()
        {
            var write = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailWith("Write what?");
            Expect(ERhoToken.CloseParan);

            write.Add(Pop());
            return Append(write);
        }

        private bool If()
        {
            var @if = NewNode(Consume());
            if (!Expression())
                return FailWith("If what?");
            @if.Add(Pop());

            // get the true-clause
            if (!Block())
                return FailWith("If needs a block");
            @if.Add(Pop());

            // if there's an else-clause, add it as well
            ConsumeNewLines();
            if (TryConsume(ERhoToken.Else))
            {
                if (!Block())
                    return FailWith("No else block");
                @if.Add(Pop());
            }

            return Append(@if);
        }

        private bool For()
        {
            var @for = NewNode(Consume());

            if (!Expression())
                return FailWith("For what?");

            if (TryConsume(ERhoToken.In))
            {
                // for (a in b) ...
                if (!ForEach(@for))
                    return false;
            }
            else
            {
                // for (a = 0; n < 10; ++a) ...
                if (!ForLoop(@for))
                    return false;
            }

            return Append(@for);
        }

        private bool ForEach(RhoAstNode @for)
        {
            @for.Add(Pop());

            if (!Expression())
                return FailWith("For each in what?");

            @for.Add(Pop());
            return true;
        }

        private bool ForLoop(RhoAstNode @for)
        {
            if (!Expression())
                return FailWith("For needs an initialiser");

            @for.Add(Pop());
            Expect(ERhoToken.Semi);

            if (!Expression())
                return FailWith("When does the for statement stop?");

            @for.Add(Pop());
            Expect(ERhoToken.Semi);

            if (!Expression())
                return FailWith("What happens when the for statement loops?");

            @for.Add(Pop());
            Expect(ERhoToken.CloseParan);

            return true;
        }
    }
}

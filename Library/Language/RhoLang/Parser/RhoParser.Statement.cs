namespace Pyro.RhoLang.Parser {
    using Lexer;

    /// <inheritdoc cref="IProcess" />
    /// <summary>
    /// Rho statements are stand-alone components made of sub-expressions.
    /// <b>NOTE</b> that Statements do not leave anything on the parsing stack:
    /// They either succeed or leave a decent contextual lexical+semantic error message.
    /// </summary>
    public partial class RhoParser {
        private bool Statement() {
            ConsumeNewLines();

            switch (Current().Type) {
                case ERhoToken.WriteLine:
                case ERhoToken.Write:
                    return Write();
                case ERhoToken.Assert:
                    return Assert();
                case ERhoToken.Return:
                case ERhoToken.Yield:
                case ERhoToken.Resume:
                case ERhoToken.Suspend: {
                        var change = NewNode(Consume());
                        if (Expression())
                            change.Add(Pop());
                        return Append(change);
                    }
                case ERhoToken.While:
                    return While();
                case ERhoToken.For:
                    return For();
                case ERhoToken.If:
                    return If();
                case ERhoToken.Class:
                case ERhoToken.Fun:
                    return NamedBlock();
                case ERhoToken.Pass:
                    PushConsume();
                    return true;
                case ERhoToken.Nop:
                    return false;
            }

            if (!Expression())
                return FailLocation("Statement or expression expected.");

            return Append(Pop()) && !Maybe(ERhoToken.Nop);
        }

        private bool NamedBlock()
            => AddNamedBlock(NewNode(Consume()));

        private bool AddNamedBlock(RhoAstNode cont) {
            var ident = Expect(ERhoToken.Ident);

            Expect(ERhoToken.OpenParan);
            var args = NewNode(ERhoAst.ArgList);
            if (Maybe(ERhoToken.Ident)) {
                args.Add(Consume());

                while (TryConsume(ERhoToken.Comma))
                    args.Add(Expect(ERhoToken.Ident));
            }

            Expect(ERhoToken.CloseParan);
            Expect(ERhoToken.NewLine);

            if (!Block())
                return FailLocation("Function block expected.");

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

        private bool While() {
            var @while = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailLocation("While what?");

            Expect(ERhoToken.CloseParan);
            @while.Add(Pop());

            if (!Block())
                return FailLocation("No While body.");

            @while.Add(Pop());
            return Append(@while);
        }

        private bool Assert() {
            var assert = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailLocation("Assert needs an expression to test.");

            Expect(ERhoToken.CloseParan);

            assert.Add(Pop());
            return Append(assert);
        }

        private bool Write() {
            var write = NewNode(Consume());
            Expect(ERhoToken.OpenParan);
            if (!Expression())
                return FailLocation("Write what?");

            Expect(ERhoToken.CloseParan);

            write.Add(Pop());
            return Append(write);
        }

        private bool If() {
            var start = _Current;

            // find our current indent level by searching for last new line
            while (start > 0 && _Tokens[start].Type != ERhoToken.NewLine)
                start--;

            // the indent level of current if
            var indent = _Current - start;

            var @if = NewNode(Consume());
            if (!Expression())
                return FailLocation("If what?");

            @if.Add(Pop());

            // get the true-clause
            if (!Block())
                return FailLocation("If needs a block.");

            @if.Add(Pop());

            while (indent >= 0) {
                if (TryConsume(ERhoToken.Else)) {
                    if (!Block())
                        return FailLocation("No else block.");

                    @if.Add(Pop());
                    return Append(@if);
                }

                if (!TryConsume(ERhoToken.Tab))
                    return Append(@if);

                indent--;
            }

            return Append(@if);
        }

        /// <summary>
        /// A for-loop. Could be one of:
        ///     for (a in b)
        ///         block
        /// or
        ///     for (a = 0; a &lt; 10; ++a)
        ///         block
        /// These are stored in the same Ast node. The way to tell the
        /// difference is by the number of children in the node.
        /// </summary>
        /// <returns>True if parsing succeeded.</returns>
        private bool For() {
            var @for = NewNode(Consume());

            Expect(ERhoToken.OpenParan);
            if (Failed)
                return false;

            // add loop variable name
            @for.Add(Expect(ERhoToken.Ident));

            if (TryConsume(ERhoToken.In)) {
                // for (a in b) ...
                if (!ForEach(@for))
                    return false;
            } else {
                _Current--;
                // for (a = 0; n < 10; ++a) ...
                if (!ForLoop(@for))
                    return false;
            }

            // always add the final part of the for-loop. for the moment,
            // RhoTranslator determines which type of for-loop it is from
            // the number of arguments in the 'for' ast-node
            @for.Add(Pop());

            // both for-loops ends with a closing paran and a block
            Expect(ERhoToken.CloseParan);
            if (Failed)
                return false;

            // add the iteration block
            if (!Block())
                return FailLocation("For block expected.");

            @for.Add(Pop());

            return Append(@for);
        }

        // for (a in [1 2 3])
        //      block
        private bool ForEach(RhoAstNode @for) =>
            Expression() || FailLocation("For each in what?");

        // for (a = 0; a < 10; ++a)
        //      block
        private bool ForLoop(RhoAstNode @for) {
            if (!Expression())
                return FailLocation("Initialisation expression expected.");

            @for.Add(Pop());
            if (!TryConsume(ERhoToken.Semi))
                return FailLocation("Semi expected.");

            if (!Expression())
                return FailLocation("When does the for statement stop?");
            @for.Add(Pop());

            Expect(ERhoToken.Semi);

            if (!Expression())
                return FailLocation("What happens at end of for loop?");

            @for.Add(Pop());

            //Expect(ERhoToken.CloseParan);

            //return Block() || FailLocation("For each in what?");
            return true;
        }
    }
}


using System;

namespace Diver.Language
{
    public partial class RhoParser
    {
        private bool Statement()
        {
            ConsumeNewLines();

            var type = Current().Type;
            switch (type)
            {
                case ERhoToken.WriteLine:
                case ERhoToken.Write:
                    return Write();

                case ERhoToken.Assert:
                    return Assert();

                case ERhoToken.Return:
                case ERhoToken.Yield:
                {
                    var ret = NewNode(Consume());
                    if (Expression())
                        ret.Add(Pop());
                    Append(ret);
                    return true;
                }

                case ERhoToken.While:
                    return While();

                case ERhoToken.For:
                    throw new NotImplementedException("For statements");

                case ERhoToken.If:
                    return If();

                case ERhoToken.Fun:
                    return Function();

                case ERhoToken.None:
                    return false;
            }

            if (!Expression())
                return FailWith("Expression expected");
            Append(Pop());

            return !Try(ERhoToken.None);
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

        private bool If()
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
            ConsumeNewLines();
            if (TryConsume(ERhoToken.Else))
            {
                if (!Block())
                    return CreateError("No else block");
                cond.Add(Pop());
            }

            Append(cond);
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
    }
}

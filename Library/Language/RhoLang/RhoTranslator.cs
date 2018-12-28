using System;
using Diver.Exec;

namespace Diver.Language
{
    public class RhoTranslator 
        : TranslatorBase<RhoParser>
    {
        public RhoTranslator(IRegistry r)
            : base(r)
        {
        }

        public bool Run(string text, EStructure st = EStructure.Expression)
        {
            _lexer = new RhoLexer(text);
            _lexer.Process();
            if (_lexer.Failed)
                return Fail(_lexer.Error);

            _parser = new RhoParser(_lexer);
            _parser.Process(_lexer, st);
            if (_parser.Failed)
                return Fail(_parser.Error);

            TranslateNode(_parser.Root);

            return !Failed;
        }

        void TranslateToken(RhoAstNode node)
        {
            switch (node.RhoToken.Type)
            {
                case ERhoToken.OpenParan:
                    foreach (var ch in node.Children)
                        TranslateNode(ch);
                    return;

                case ERhoToken.Not:
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.Not);
                    return;

                case ERhoToken.True:
                    Append(true);
                    return;

                case ERhoToken.False:
                    Append(false);
                    return;

                case ERhoToken.Assert:
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.Assert);
                    return;

                case ERhoToken.While:
                    TranslateWhile(node);
                    return;

                //case ERhoToken.DivAssign:
                //    TranslateBinaryOp(node, EOperation.DivEquals);
                //    return;

                //case ERhoToken.MulAssign:
                //    TranslateBinaryOp(node, EOperation.MulEquals);
                //    return;

                //case ERhoToken.MinusAssign:
                //    TranslateBinaryOp(node, EOperation.MinusEquals);
                //    return;

                //case ERhoToken.PlusAssign:
                //    TranslateBinaryOp(node, EOperation.PlusEquals);
                //    return;

                case ERhoToken.Assign:
                    TranslateBinaryOp(node, EOperation.Assign);
                    return;

                case ERhoToken.Retrieve:
                    Append(EOperation.Retrieve);
                    return;

                case ERhoToken.Self:
                    Append(EOperation.Self);
                    return;

                case ERhoToken.NotEquiv:
                    TranslateBinaryOp(node, EOperation.NotEquiv);
                    return;

                case ERhoToken.Equiv:
                    TranslateBinaryOp(node, EOperation.Equiv);
                    return;

                case ERhoToken.Less:
                    TranslateBinaryOp(node, EOperation.Less);
                    return;

                case ERhoToken.Greater:
                    TranslateBinaryOp(node, EOperation.Greater);
                    return;

                case ERhoToken.GreaterEquiv:
                    TranslateBinaryOp(node, EOperation.GreaterOrEquiv);
                    return;

                case ERhoToken.LessEquiv:
                    TranslateBinaryOp(node, EOperation.LessOrEquiv);
                    return;

                case ERhoToken.Minus:
                    TranslateBinaryOp(node, EOperation.Minus);
                    return;

                case ERhoToken.Plus:
                    TranslateBinaryOp(node, EOperation.Plus);
                    return;

                case ERhoToken.Multiply:
                    TranslateBinaryOp(node, EOperation.Multiply);
                    return;

                case ERhoToken.Divide:
                    TranslateBinaryOp(node, EOperation.Divide);
                    return;

                case ERhoToken.Or:
                    TranslateBinaryOp(node, EOperation.LogicalOr);
                    return;

                case ERhoToken.And:
                    TranslateBinaryOp(node, EOperation.LogicalAnd);
                    return;

                case ERhoToken.Int:
                    Append(int.Parse(node.Text));
                    return;

                case ERhoToken.Float:
                    Append(float.Parse(node.Text));
                    return;

                case ERhoToken.String:
                    Append(node.Text);
                    return;

                case ERhoToken.Ident:
                    Append(new Label(node.Text));
                    return;

                case ERhoToken.Pathname:
                    //Append(new Pathname(node.Token.Text));
                    throw new NotImplementedException("Translate pathname");
                    return;

                case ERhoToken.Yield:
                    //for (var ch : node.Children)
                    //    Translate(ch);
                    Append(EOperation.Suspend);
                    return;

                case ERhoToken.Return:
                    foreach (var ch in node.Children)
                        TranslateNode(ch);
                    Append(EOperation.Resume);
                    return;
            }

            Fail($"Unsupported node {node}");
        }

        void TranslateBinaryOp(RhoAstNode node, EOperation op)
        {
            TranslateNode(node.GetChild(0));
            TranslateNode(node.GetChild(1));

            Append(op);
        }

        //void TranslatePathname(RhoAstNode node)
        //{
        //    Pathname::Elements elements;
        //    typedef Pathname::Element El;
        //
        //    for (var ch : node.GetChildren())
        //    {
        //        switch (ch.GetToken().type)
        //        {
        //        case RhoTokenEnumType::Quote:
        //            elements.push_back(El::Quote);
        //            break;
        //        case RhoTokenEnumType::Sep:
        //            elements.push_back(El::Separator);
        //            break;
        //        case RhoTokenEnumType::Ident:
        //            elements.push_back(Label(ch.GetTokenText()));
        //            break;
        //        }
        //    }
        //
        //    AppendNew(Pathname(move(elements)));
        //}


        protected void TranslateNode(RhoAstNode node)
        {
            if (node == null)
            {
                Fail("Unexpected empty node");
                return;
            }

            switch (node.Type)
            {
                case ERhoAst.Pathname:
                    TranslateToken(node);
                    return;

                case ERhoAst.IndexOp:
                    TranslateBinaryOp(node, EOperation.At);
                    return;

                case ERhoAst.GetMember:
                    TranslateBinaryOp(node, EOperation.GetProperty);
                    return;

                case ERhoAst.TokenType:
                    TranslateToken(node);
                    return;

                case ERhoAst.Assignment:
                    // like a binary op, but argument order is reversed
                    TranslateNode(node.GetChild(1));
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.Store);
                    return;

                case ERhoAst.Call:
                    TranslateCall(node);
                    return;

                case ERhoAst.Conditional:
                    TranslateIf(node);
                    return;

                case ERhoAst.Block:
                    PushNew();
                    foreach (var st in node.Children)
                        TranslateNode(st);
                    Append(Pop());
                    return;

                case ERhoAst.List:
                    //KAI_NOT_IMPLEMENTED();
                    throw new NotImplementedException();
                    return;

                case ERhoAst.For:
                    TranslateFor(node);
                    return;

                case ERhoAst.Function:
                    TranslateFunction(node);
                    return;

                case ERhoAst.Program:
                    foreach (var e in node.Children)
                        TranslateNode(e);
                    return;
            }

            Fail($"Unsupported node {node}");
        }

        private void TranslateBlock(RhoAstNode node)
        {
            foreach (var st in node.Children)
                TranslateNode(st);
        }

        private void TranslateFunction(RhoAstNode node)
        {
            // child 0: ident
            // child 1: args
            // child 2: block
            var ch = node.Children;

            // write the body
            PushNew();
            foreach (var b in ch[2].Children)
                TranslateNode(b);

            // add the args
            var cont = Pop();
            foreach (var arg in ch[1].Children)
                cont.AddArg(arg.Token.Text);

            // write the name and store
            Append(cont);
            Append(ch[0].Text);
            Append(EOperation.Store);
        }

        private void TranslateCall(RhoAstNode node)
        {
            var children = node.Children;
            foreach (var a in children[1].Children)
                TranslateNode(a);

            TranslateNode(children[0]);
            if (children.Count > 2 && children[2].Token.Type == ERhoToken.Replace)
                Append(EOperation.Replace);
            else
                Append(EOperation.Suspend);
        }

        private void TranslateIf(RhoAstNode node)
        {
            var ch = node.Children;
            TranslateNode(ch[0]);
            var hasElse = ch.Count > 2;
            if (hasElse)
                TranslateNode(ch[2]);

            TranslateNode(ch[1]);
            Append(hasElse ? EOperation.If : EOperation.IfElse);
        }

        static void TranslateFor(RhoAstNode node)
        {
            throw new NotImplementedException("for loops");
        }

        static void TranslateWhile(RhoAstNode node)
        {
            throw new NotImplementedException("while loops");
        }

        public override string ToString()
        {
            return $"=== RhoTranslator:\nInput: {_lexer.Input}Lexer: {_lexer}\nParser: {_parser}";
        }

        private RhoLexer _lexer;
        private RhoParser _parser;
    }
}

using System;
using System.Linq;
using Diver.Exec;

namespace Diver.Language
{
    public class RhoTranslator 
        : TranslatorBase<RhoLexer, RhoParser>
    {
        public RhoTranslator(IRegistry r)
            : base(r)
        {
        }

        void ShowTime(string name, Action action)
        {
            var start = DateTime.Now;
            action();
            WriteLine($"{name} took {(DateTime.Now - start).TotalMilliseconds}");
        }

        public override bool Translate(string text, EStructure st = EStructure.Program)
        {
            _lexer = new RhoLexer(text);
            ShowTime("Lexer", () => _lexer.Process());
            if (_lexer.Failed)
                return Fail(_lexer.Error);

            _parser = new RhoParser(_lexer, _reg, st);
            ShowTime("Parser", () => _parser.Process());
            if (_parser.Failed)
                return Fail(_parser.Error);

            //ShowTime("PrintTree", () => WriteLine(_parser.PrintTree()));
            ShowTime("Translator", () => TranslateNode(_parser.Result));

            return !Failed;
        }

        void TranslateToken(RhoAstNode node)
        {
            switch (node.RhoToken.Type)
            {
                case ERhoToken.Fun:
                    TranslateFunction(node);
                    return;

                case ERhoToken.Assert:
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.Assert);
                    return;

                 case ERhoToken.If:
                     TranslateIf(node);
                     return;

                case ERhoToken.Write:
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.Write);
                    return;

                case ERhoToken.WriteLine:
                    TranslateNode(node.GetChild(0));
                    Append(EOperation.WriteLine);
                    return;

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
                    TranslateNode(node.GetChild(0));
                    AddQuoted(node.GetChild(1));
                    Append(EOperation.Assign);
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

                case ERhoToken.Xor:
                    TranslateBinaryOp(node, EOperation.LogicalXor);
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

        private void AddQuoted(RhoAstNode node)
        {
            Append(new Label(node.Text, true));
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
                case ERhoAst.Suspend:
                    Append(EOperation.Suspend);
                    break;

                case ERhoAst.Pathname:
                    TranslateToken(node);
                    return;

                case ERhoAst.Assignment:
                    // like a binary op, but argument order is reversed
                    //TranslateNode(node.GetChild(1));
                    TranslateNode(node.GetChild(0));
                    AddQuoted(node.GetChild(1));
                    Append(EOperation.Store);
                    return;

                case ERhoAst.IndexOp:
                    TranslateBinaryOp(node, EOperation.At);
                    return;

                //case ERhoAst.Assignment:
                //    // like a binary op, but argument order is reversed
                //    //TranslateNode(node.GetChild(1));
                //    TranslateNode(node.GetChild(0));
                //    Append(node.GetChild(1).Value);
                //    Append(EOperation.Store);
                //    return;

                case ERhoAst.Call:
                    TranslateCall(node);
                    return;

                case ERhoAst.GetMember:
                    TranslateGetMember(node);
                    return;


                case ERhoAst.Conditional:
                    TranslateIf(node);
                    return;

                case ERhoAst.Block:
                    PushNew();
                    TranslateBlock(node);
                    Append(Pop());
                    return;

                case ERhoAst.List:
                    //KAI_NOT_IMPLEMENTED();
                    throw new NotImplementedException();
                    return;

                case ERhoAst.For:
                    TranslateFor(node);
                    return;

                case ERhoAst.Program:
                    TranslateBlock(node);
                    return;
                default:
                    TranslateToken(node);
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
            var ch = node.Children;
            var ident = ch[0].Value;
            var args = ch[1].Children;
            var block = ch[2].Children;

            // write the body
            PushNew();
            foreach (var obj in block)
                TranslateNode(obj);
            var cont = Pop();

            // add the args
            foreach (var arg in args)
                cont.AddArg(arg.Token.Text);

            Append(cont);
       }

        private void TranslateCall(RhoAstNode node)
        {
            var children = node.Children;
            var args = children[1].Children;
            var name = children[0];
            foreach (var a in args.Reverse())
                TranslateNode(a);

            TranslateNode(name);
            // TODO: add Replace/Suspend/Resume to children
            if (children.Count > 2 && children[2].Token.Type == ERhoToken.Replace)
                Append(EOperation.Replace);
            else
                Append(EOperation.Suspend);
        }

        private void TranslateGetMember(RhoAstNode node)
        {
            var ch = node.Children;
            var subject = ch[0];
            var member = ch[1];
            var ident = new Label(member.Text, true);
            Append(ident);
            TranslateNode(subject);
            Append(EOperation.GetMember);
        }

        private void TranslateIf(RhoAstNode node)
        {
            var ch = node.Children;
            var test = ch[0];
            var thenBlock = ch[1];
            var elseBlock = ch.Count > 2 ? ch[2] : null;
            var hasElse = elseBlock != null;

            TranslateNode(thenBlock);
            if (hasElse)
                TranslateNode(elseBlock);
            TranslateNode(test);
            Append(hasElse ? EOperation.IfElse : EOperation.If);

            // TODO: Allow for if! and if... as well as if&
            Append(EOperation.Suspend);
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
            return $"=== RhoTranslator:\n--- Input: {_lexer.Input}--- Lexer: {_lexer}\n--- Parser: {_parser}\n--- Code: {Result().ToString()}";
        }
    }
}

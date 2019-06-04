using System;
using System.Linq;

namespace Pyro.RhoLang
{
    using Exec;
    using Lexer;
    using Parser;

    using Language;
    using Language.Parser;
    using Language.Impl;

    /// <summary>
    /// Translate from text to an executable Continuation.
    /// </summary>
    public class RhoTranslator 
        : TranslatorBase<RhoLexer, RhoParser>
    {
        public RhoTranslator(IRegistry r)
            : base(r)
        {
        }

        public override string ToString()
            => $"=== RhoTranslator:\n--- Input: {_Lexer.Input}--- Lexer: {_Lexer}\n--- Parser: {_Parser}\n--- Code: {Result}";

        public override bool Translate(string text, out Continuation result, EStructure st = EStructure.Program)
        {
            if (!base.Translate(text, out result, st))
                return false;

            if (string.IsNullOrEmpty(text))
                return true;

            _Lexer = new RhoLexer(text);
            _Lexer.Process();
            if (_Lexer.Failed)
                return Fail(_Lexer.Error);

            _Parser = new RhoParser(_Lexer, _reg, st);
            _Parser.Process();
            if (_Parser.Failed)
                return Fail(_Parser.Error);

            WriteLine(_Parser.PrintTree());
            Node(_Parser.Result);

            result = Result;
            //WriteLine($"{this}");
            return !Failed;
        }

        private void ShowTime(string name, Action action)
        {
            var start = DateTime.Now;
            action();
            WriteLine($"{name} took {(DateTime.Now - start).TotalMilliseconds}");
        }

        private bool Token(RhoAstNode node)
        {
            switch (node.RhoToken.Type)
            {
                case ERhoToken.Fun:
                    return Function(node);

                case ERhoToken.Assert:
                    Node(node.GetChild(0));
                    return Append(EOperation.Assert);

                 case ERhoToken.If:
                     return If(node);

                case ERhoToken.Write:
                    Node(node.GetChild(0));
                    return Append(EOperation.Write);

                case ERhoToken.WriteLine:
                    Node(node.GetChild(0));
                    return Append(EOperation.WriteLine);

                case ERhoToken.OpenParan:
                    return node.Children.All(Node);

                case ERhoToken.Not:
                    Node(node.GetChild(0));
                    return Append(EOperation.Not);

                case ERhoToken.True:
                    return Append(true);

                case ERhoToken.False:
                    return Append(false);

                case ERhoToken.While:
                    return While(node);

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
                    Node(node.GetChild(0));
                    AppendQuoted(node.GetChild(1));
                    return Append(EOperation.Assign);

                case ERhoToken.Retrieve:
                    return Append(EOperation.Retrieve);

                case ERhoToken.Self:
                    return Append(EOperation.Self);

                case ERhoToken.NotEquiv:
                    return BinaryOp(node, EOperation.NotEquiv);

                case ERhoToken.Equiv:
                    return BinaryOp(node, EOperation.Equiv);

                case ERhoToken.Less:
                    return BinaryOp(node, EOperation.Less);

                case ERhoToken.Greater:
                    return BinaryOp(node, EOperation.Greater);

                case ERhoToken.GreaterEquiv:
                    return BinaryOp(node, EOperation.GreaterOrEquiv);

                case ERhoToken.LessEquiv:
                    return BinaryOp(node, EOperation.LessOrEquiv);

                case ERhoToken.Minus:
                    return BinaryOp(node, EOperation.Minus);

                case ERhoToken.Plus:
                    return BinaryOp(node, EOperation.Plus);

                case ERhoToken.Multiply:
                    return BinaryOp(node, EOperation.Multiply);

                case ERhoToken.Divide:
                    return BinaryOp(node, EOperation.Divide);

                case ERhoToken.Or:
                    return BinaryOp(node, EOperation.LogicalOr);

                case ERhoToken.And:
                    return BinaryOp(node, EOperation.LogicalAnd);

                case ERhoToken.Xor:
                    return BinaryOp(node, EOperation.LogicalXor);

                case ERhoToken.Int:
                    return Append(int.Parse(node.Text));

                case ERhoToken.Float:
                    return Append(float.Parse(node.Text));

                case ERhoToken.String:
                    return Append(node.Text);

                case ERhoToken.Ident:
                    return Append(new Label(node.Text));

                case ERhoToken.Pathname:
                    throw new NotImplementedException("Translate pathname");

                case ERhoToken.Yield:
                    return Append(EOperation.Suspend);

                case ERhoToken.Return:
                    foreach (var ch in node.Children)
                        Node(ch);
                    return Append(EOperation.Resume);

                case ERhoToken.For:
                    return For(node);

                case ERhoToken.PiSlice:
                    return PiSlice(node);
            }

            return Fail($"Unsupported Token {node.Token.Type}");
        }

        private bool PiSlice(RhoAstNode rhoNode)
        {
            if (!(rhoNode.Value is PiAstNode piNode))
                return Fail("Internal error: PiAstNode type expected");

            // TODO: store a private _piTranslator that is re-used
            return new PiTranslator(_reg).TranslateNode(piNode, Top().Code)
                || Fail("Couldn't translate pi");
        }

        private void AppendQuoted(RhoAstNode node)
            => Append(new Label(node.Text, true));

        private bool BinaryOp(RhoAstNode node, EOperation op)
        {
            Node(node.GetChild(0));
            Node(node.GetChild(1));

            return Append(op);
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


        // TODO: have Append() etc, and this method, return bool
        protected bool Node(RhoAstNode node)
        {
            if (node == null)
                return Fail("INTERNAL: Unexpected empty node");

            switch (node.Type)
            {
                case ERhoAst.Suspend:
                    return Append(EOperation.Suspend);

                case ERhoAst.Pathname:
                    return Token(node);

                case ERhoAst.Assignment:
                    Node(node.GetChild(0));
                    AppendQuoted(node.GetChild(1));
                    return Append(EOperation.Store);

                case ERhoAst.IndexOp:
                    return BinaryOp(node, EOperation.At);

                case ERhoAst.Call:
                    return Call(node);

                case ERhoAst.GetMember:
                    return GetMember(node);

                case ERhoAst.Conditional:
                    return If(node);

                case ERhoAst.Block:
                    PushNew();
                    Block(node);
                    return Append(Pop());

                case ERhoAst.List:
                    return List(node);

                case ERhoAst.For:
                    return For(node);

                case ERhoAst.Program:
                    return Block(node);

                default:
                    return Token(node);
            }

            //return Fail($"Unsupported RhoAstNode {node}");
        }

        private bool List(RhoAstNode node)
        {
            PushNew();
            foreach (var ch in node.Children)
                Node(ch);

            return Append(Pop().Code);
        }

        private bool Block(RhoAstNode node)
        {
            foreach (var st in node.Children)
                Node(st);

            return true;
        }

        private bool Function(RhoAstNode node)
        {
            var ch = node.Children;
            var args = ch[1].Children;
            var block = ch[2].Children;

            PushNew();
            foreach (var obj in block)
                Node(obj);

            var cont = Pop();
            foreach (var arg in args)
                cont.AddArg(arg.Token.Text);

            return Append(cont);
       }

        private bool Call(RhoAstNode node)
        {
            var children = node.Children;
            var args = children[1].Children;
            var name = children[0];
            foreach (var a in args.Reverse())
                Node(a);

            Node(name);
            // TODO: add Replace/Suspend/Resume to children
            if (children.Count > 2 && children[2].Token.Type == ERhoToken.Replace)
                Append(EOperation.Replace);
            else
                Append(EOperation.Suspend);

            return true;
        }

        private bool GetMember(RhoAstNode node)
        {
            var ch = node.Children;
            var subject = ch[0];
            var member = ch[1];
            
            AppendQuoted(member);
            Node(subject);

            return Append(EOperation.GetMember);
        }

        private bool If(RhoAstNode node)
        {
            var ch = node.Children;
            var test = ch[0];
            var thenBlock = ch[1];
            var elseBlock = ch.Count > 2 ? ch[2] : null;
            var hasElse = elseBlock != null;

            Node(thenBlock);
            if (hasElse)
                Node(elseBlock);

            Node(test);
            Append(hasElse ? EOperation.IfElse : EOperation.If);

            // TODO: Allow for if! and if... as well as if&
            return Append(EOperation.Suspend);
        }

        private bool For(RhoAstNode node)
        {
            var ch = node.Children;
            if (ch.Count == 3)
            {
                // for (a in b) ...
                AppendQuoted(ch[0]);
                Node(ch[1]);
                Node(ch[2]);
                return Append(EOperation.ForEachIn);
            }

            // for (a = 0; a < 10; ++a) ...
            Node(ch[0]);
            Node(ch[1]);
            Node(ch[2]);
            Node(ch[3]);
            return Append(EOperation.ForLoop);
        }

        private static bool While(RhoAstNode node)
            => throw new NotImplementedException("while loops");
    }
}


using System.CodeDom.Compiler;

namespace Pyro.RhoLang
{
    using System;
    using System.Linq;
    using Exec;
    using Lexer;
    using Parser;
    using Language;
    using Language.Parser;
    using Language.Impl;

    /// <inheritdoc />
    /// <summary>
    /// Translate from Rho script to an executable Continuation.
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

            if (!Generate(_Parser.Result))
                return false;

            result = Result;
            return !Failed;
        }

        /// <summary>
        /// Translate given node into pi-code.
        /// </summary>
        private bool Token(RhoAstNode node)
        {
            switch (node.RhoToken.Type)
            {
                case ERhoToken.New:
                    return GenNew(node);
                case ERhoToken.Assign:
                    return Assign(node);
                case ERhoToken.Fun:
                case ERhoToken.Class:
                    return Function(node);
                case ERhoToken.Assert:
                    return AppendChildOp(node, EOperation.Assert);
                case ERhoToken.If:
                     return If(node);
                case ERhoToken.Write:
                    return AppendChildOp(node, EOperation.Write);
                case ERhoToken.WriteLine:
                    return AppendChildOp(node, EOperation.WriteLine);
                case ERhoToken.OpenParan:
                    return node.Children.All(Generate);
                case ERhoToken.Not:
                    return AppendChildOp(node, EOperation.Not);
                case ERhoToken.True:
                    return Append(true);
                case ERhoToken.False:
                    return Append(false);
                case ERhoToken.While:
                    return While(node);
                case ERhoToken.DivAssign:
                    return BinaryOp(node, EOperation.DivEquals);
                case ERhoToken.MulAssign:
                    return BinaryOp(node, EOperation.MulEquals);
                case ERhoToken.MinusAssign:
                    return BinaryOp(node, EOperation.MinusEquals);
                case ERhoToken.PlusAssign:
                    return BinaryOp(node, EOperation.PlusEquals);
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
                    // DO NOT REFACTOR INTO LINQ.
                    // In fact, don't use Linq anywhere in this library.
                    foreach (var child in node.Children)
                        if (!Generate(child))
                            return false;
                    return Append(EOperation.Resume);
                case ERhoToken.For:
                    return For(node);
                case ERhoToken.PiSlice:
                    return PiSlice(node);
            }

            return Fail($"Unsupported RhoToken {node.Token.Type}");
        }

        private bool GenNew(RhoAstNode node)
        {
            Append(node.Children[0].Text);
            return Append(EOperation.New);
        }

        private bool AppendChildOp(RhoAstNode node, EOperation op)
            => Generate(node.GetChild(0)) && Append(op);

        private bool AppendQuoted(RhoAstNode node)
            => Append(new Label(node.Text, true));

        private bool List(RhoAstNode node)
            => PushNew() && GenerateChildren(node) && Append(Pop().Code);

        private bool Block(RhoAstNode node)
            => GenerateChildren(node);

        private bool PiSlice(RhoAstNode rhoNode)
        {
            if (!(rhoNode.Value is PiAstNode piNode))
                return InternalFail("PiAstNode type expected");

            // TODO: store a private _piTranslator that is re-used
            return new PiTranslator(_reg).TranslateNode(piNode, Top().Code)
                || Fail("Couldn't translate pi");
        }

        private bool BinaryOp(RhoAstNode node, EOperation op)
        {
            Generate(node.GetChild(0));
            Generate(node.GetChild(1));

            return Append(op);
        }

        /// <summary>
        /// Generate executable pi-code from given node.
        /// </summary>
        protected bool Generate(RhoAstNode node)
        {
            if (node == null)
                return InternalFail("Unexpected empty RhoAstNode");

            switch (node.Type)
            {
                case ERhoAst.Suspend:
                    return Append(EOperation.Suspend);

                case ERhoAst.Pathname:
                    return Token(node);

                case ERhoAst.Assignment:
                    Generate(node.GetChild(0));
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
                    return PushNew() && Block(node) && Append(Pop());
                case ERhoAst.List:
                    return List(node);
                case ERhoAst.For:
                    return For(node);
                case ERhoAst.Program:
                    return Block(node);
                default:
                    return Token(node);
            }
        }

        private bool GenerateChildren(RhoAstNode node)
        {
            foreach (var st in node.Children)
                if (!Generate(st))
                    return InternalFail($"Failed to generate code for '{st}' from {node}");

            return true;
        }

        private bool Assign(RhoAstNode node)
        {
            var ch = node.Children;
            Generate(ch[0]);    // the r-value

            switch (ch[1].Type)
            {
            case ERhoAst.TokenType:
                switch (ch[1].Token.Type)
                {
                case ERhoToken.Ident:
                    return AppendQuoted(ch[1]) && Append(EOperation.Assign);
                }
                break;

            case ERhoAst.GetMember:
                ch = ch[1].Children;
                var subject = ch[0];
                var member = ch[1];

                AppendQuoted(member);
                Generate(subject);

                return Append(EOperation.SetMember);
            }

            return false;
        }

        private bool Function(RhoAstNode node)
        {
            var ch = node.Children;
            var args = ch[1].Children;
            var block = ch[2].Children;

            PushNew();
            foreach (var obj in block)
                Generate(obj);

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
                Generate(a);

            Generate(name);

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
            Generate(subject);

            return Append(EOperation.GetMember);
        }

        private bool If(RhoAstNode node)
        {
            var ch = node.Children;
            var test = ch[0];
            var thenBlock = ch[1];
            var elseBlock = ch.Count > 2 ? ch[2] : null;
            var hasElse = elseBlock != null;

            Generate(thenBlock);
            if (hasElse)
                Generate(elseBlock);

            Generate(test);
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
                Generate(ch[1]);
                Generate(ch[2]);
                return Append(EOperation.ForEachIn);
            }

            // for (a = 0; a < 10; ++a) ...
            Generate(ch[0]);
            Generate(ch[1]);
            Generate(ch[2]);
            Generate(ch[3]);
            return Append(EOperation.ForLoop);
        }

        private static bool While(RhoAstNode node)
            => throw new NotImplementedException("while loops");
    }
}


using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Pyro.Language.Impl {
    /// <inheritdoc />
    /// <summary>
    /// Common for all Parsers.
    /// Iterate over a stream of tokens to produce an abstract syntax tree
    /// </summary>
    public class ParserCommon<TLexer, TAstNode, TTokenNode, ETokenEnum, EAstEnum, TAstFactory>
        : ProcessCommon
            where TLexer
                : ILexerCommon<TTokenNode>
            where TAstFactory
                : class, IAstFactory<TTokenNode, TAstNode, EAstEnum>, new()
            where TTokenNode
                : class, ITokenNode<ETokenEnum>
            where TAstNode
                : class {
        
        protected readonly List<TTokenNode> _Tokens = new List<TTokenNode>();

        /// <summary>
        /// The runtime stack of AstNodes. This will change as
        /// the parser operates.
        /// </summary>
        protected readonly Stack<TAstNode> _Stack = new Stack<TAstNode>();

        /// <summary>
        /// The current offset into the input string that is being Tokenised
        /// </summary>
        protected int _Current;

        protected TLexer _Lexer;

        protected TAstFactory _AstFactory = new TAstFactory();

        protected ParserCommon(TLexer lexer, IRegistry reg)
            : base(reg) {
            _Current = 0;
            _Lexer = lexer;
        }

        public string PrintTree() {
            if (_Stack.Count == 0) {
                return "[Empty]";
            }

            var str = new StringBuilder();
            PrintTree(str, 0, _Stack.Peek());
            return str.ToString();
        }

        private void PrintTree(StringBuilder str, int level, TAstNode root) {
            var val = root.ToString();
            if (string.IsNullOrEmpty(val)) {
                return;
            }

            for (var n = 0; n < level; ++n) {
                str.Append("  ");
            }

            str.Append(val);
            str.Append(Environment.NewLine);
            foreach (var ch in _AstFactory.GetChildren(root))
                PrintTree(str, level + 1, ch);
        }

        public override string ToString()
            => PrintTree();

        private bool HasTokens()
            => _Current < _Tokens.Count;

        protected TAstNode Pop()
            => !StackNotEmpty() ? null : _Stack.Pop();

        protected TAstNode Top()
            => !StackNotEmpty() ? null : _Stack.Peek();

        private bool StackNotEmpty()
            => _Stack.Count > 0 || FailLocation("Empty context stack");

        protected bool Push(TAstNode node) {
            if (node == null) {
                throw new NullValueException();
            }

            _Stack.Push(node);
            return true;
        }

        protected bool Append(TAstNode obj) {
            if (obj == null) {
                return FailLocation("Cannot add Null object to internal parse stack");
            }

            if (Top() == null) {
                Push(obj);
            } else {
                _AstFactory.AddChild(Top(), obj);
            }

            return true;
        }

        protected TAstNode PushConsume() {
            var node = NewNode(Consume());
            Push(node);
            return node;
        }

        protected bool PushConsumed() {
            PushConsume();
            return true;
        }

        private TTokenNode Next() {
            if (_Current != _Tokens.Count) {
                return _Tokens[++_Current];
            }

            FailLocation("Expected more");
            throw new Exception("Expected more");
        }

        protected TTokenNode Current() {
            if (HasTokens()) {
                return _Tokens[_Current];
            }

            FailLocation("Expected something more");
            throw new Exception(_Lexer.CreateErrorMessage(Current(), @"Unexpected end of text"));
        }

        protected TTokenNode Consume() {
            if (_Current != _Tokens.Count) {
                return _Tokens[_Current++];
            }

            FailLocation("Expected something more");
            throw new NotImplementedException("Expected something");
        }

        protected TAstNode Expect(ETokenEnum type) {
            var tok = Current();
            if (!type.Equals(tok.Type)) {
                FailLocation($"Expected {type}, have {tok}");
            } else {
                Next();
            }

            return _AstFactory.New(Last());
        }

        protected bool Empty()
            => _Current >= _Tokens.Count;

        private TTokenNode Last()
            => _Tokens[_Current - 1];

        protected TTokenNode Peek()
            => _Current + 1 >= _Tokens.Count ? null : _Tokens[_Current + 1];

        protected bool Try(IList<ETokenEnum> types)
            => Enumerable.Contains(types, Current().Type);

        protected bool Maybe(ETokenEnum type)
            => !Empty() && Current().Type.Equals(type);

        protected bool FailLocation(string text)
            => Fail(!HasTokens() ? text : _Lexer.CreateErrorMessage(Current(), text));

        protected TAstNode NewNode(EAstEnum a)
            => _AstFactory.New(a);

        protected TAstNode NewNode(TTokenNode t)
            => _AstFactory.New(t);
    }
}

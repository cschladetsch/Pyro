using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diver.Language
{
    // common for all parsers.
    // iterate over a stream of tokens to produce an abstract syntax tree
    public class ParserCommon<TLexer, TAstNode, TTokenNode, ETokenEnum, EAstEnum, AstFactory>
        : ProcessCommon
        where TLexer : ILexerCommon<TTokenNode>
        where AstFactory : class, IAstFactory<TTokenNode, TAstNode, EAstEnum>, new()
        where TTokenNode : class, ITokenNode<ETokenEnum>
        where TAstNode : class//, IAstNode<TAstNode>
    {
        public TAstNode Root => _root;

        protected ParserCommon(LexerBase lexer, IRegistry r) : base(r)
        {
            _current = 0;
            _indent = 0;
        }

        public virtual bool Process(TLexer lex, EStructure structure)
        {
            return false;
        }

        protected virtual bool Process()
        {
            return false;
        }

        protected bool Run(EStructure st)
        {
            try
            {
                Process(_lexer, st);
            }
            catch (Exception e)
            {
                if (!Failed)
                    Fail(_lexer.CreateErrorMessage(Current(), "%s", e.ToString()));
            }

            return !Failed;
        }

        public string PrintTree()
        { 
            var str = new StringBuilder();
            PrintTree(str, 0, _root);
            return str.ToString();
        }

        public override string ToString()
        {
            return PrintTree();
        }

        private void PrintTree(StringBuilder str, int level, TAstNode root)
        {
            var val = root.ToString();
            if (string.IsNullOrEmpty(val))
                return;
            for (int n = 0; n < level; ++n)
                str.Append("    ");

            str.Append(val);
            str.Append(Environment.NewLine);
            foreach (var ch in _astFactory.GetChildren(root))
            {
                PrintTree(str, level + 1, ch);
            }
        }

        protected bool Has()
        {
            return _current < _tokens.Count;
        }

        protected void Push(TAstNode node)
        {
            if (node != null)
                _stack.Push(node);
        }

        protected void Append(object obj)
        {
            _astFactory.AddChild(Top(), obj);
        }

        protected TAstNode Pop()
        {
            if (_stack.Count == 0)
            {
                FailWith("Empty context stack");
                throw new ContextStackEmptyException();
            }

            return _stack.Pop();
        }

        protected TAstNode Top()
        {
            return _stack.Peek();
        }

        protected bool PushConsume()
        {
            Push(NewNode(Consume()));
            return true;
        }

        protected TTokenNode Next()
        {
            if (_current == _tokens.Count)
            {
                FailWith("Expected more");
                throw new Exception("Expected more");
            }

            return _tokens[++_current];
        }

        protected TTokenNode Last()
        {
            return _tokens[_current - 1];
        }

        protected TTokenNode Current()
        {
            if (!Has())
            {
                FailWith("Expected something more");
                throw new Exception("Expected something");
            }

            return _tokens[_current];
        }

        protected bool Current(TTokenNode node)
        {
            return _current < _tokens.Count && _tokens[_current].Equals(node);
        }

        protected bool Empty()
        {
            return _current >= _tokens.Count;
        }

        protected TTokenNode Peek()
        {
            return _current + 1 >= _tokens.Count ? null : _tokens[_current + 1];
        }

        protected bool PeekConsume(ETokenEnum ty)
        {
            if (!Peek().Type.Equals(ty))
                return false;
            Consume();
            return true;

        }

        protected bool CurrentIs(ETokenEnum ty)
        {
            return Current().Type.Equals(ty);
        }

        protected bool PeekIs(ETokenEnum ty)
        {
            return Peek().Type.Equals(ty);
        }

        protected bool Consume(ETokenEnum ty)
        {
            if (!Current().Type.Equals(ty))
                return false;
            Consume();
            return true;

        }

        protected TTokenNode Consume()
        {
            if (_current == _tokens.Count)
            {
                //KAI_TRACE_ERROR_1(Fail("Unexpected end of file"));
                //KAI_THROW_1(LogicError, "Expected something");
                FailWith("Expected something more");
                throw new NotImplementedException("Expected something");
            }

            return _tokens[_current++];
        }

        protected bool Try(IList<ETokenEnum> types)
        {
            return Enumerable.Contains(types, Current().Type);
        }

        protected bool Try(ETokenEnum type)
        {
            return !Empty() && Current().Type.Equals(type);
        }

        protected bool FailWith(string text)
        {
            return Fail(_lexer.CreateErrorMessage(Current(), text));
        }

        protected TAstNode Expect(ETokenEnum type)
        {
            var tok = Current();
            if (!tok.Type.Equals(type))
            {
                FailWith($"Expected {type}, have {tok}");
                return null;
            }

            Next();
            return _astFactory.New(Last());
        }

        protected TAstNode NewNode(EAstEnum t)
        {
            return _astFactory.New(t);
        }

        protected TAstNode NewNode(EAstEnum e, TTokenNode t)
        {
            return _astFactory.New(e, t);
        }

        protected TAstNode NewNode(TTokenNode t)
        {
            return _astFactory.New(t);
        }

        protected List<TTokenNode> _tokens = new List<TTokenNode>();
        protected readonly Stack<TAstNode> _stack = new Stack<TAstNode>();
        protected int _current;
        protected TAstNode _root;
        protected int _indent;
        protected TLexer _lexer;
        protected AstFactory _astFactory = new AstFactory();
    }
}


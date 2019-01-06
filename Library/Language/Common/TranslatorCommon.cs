using System.Collections.Generic;
using System.Text;
using Diver.Exec;

namespace Diver.Language
{
    /// <summary>
    /// Common to all processes that translate an AST to Pi code sequences.
    /// </summary>
    public class TranslatorCommon
        : ProcessCommon
    {
        public int TraceLevel;

        protected TranslatorCommon(IRegistry r)
            : base(r)
        {
            _stack.Push(new Continuation(new List<object>()));
        }

        public virtual bool Translate(string text, EStructure st = EStructure.Program)
        {
            return false;
        }

        protected new void Reset()
        {
            base.Reset();
            _stack.Clear();
            _stack.Push(new Continuation(new List<object>()));
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var cont in _stack)
            {
                str.Append(cont.ToString());
            }

            return str.ToString();
        }

        public virtual Continuation Result()
        {
            return _stack.Count == 0 ? null : Top();
        }

        protected void PushNew()
        {
            _stack.Push(new Continuation(new List<object>()));
        }

        protected Continuation Pop()
        {
            return _stack.Pop();
        }

        protected Continuation Top()
        {
            return _stack.Peek();
        }

        protected void Append(object obj)
        {
            Top().Code.Add(obj);
        }

        private readonly Stack<Continuation> _stack = new Stack<Continuation>();
    }
}

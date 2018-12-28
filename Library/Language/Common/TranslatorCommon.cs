using System.Collections.Generic;
using System.Text;
using Diver.Exec;

namespace Diver.Language
{
    public class TranslatorCommon
        : ProcessCommon
    {
        protected TranslatorCommon(IRegistry r)
            : base(r)
        {
        }

        protected virtual Continuation Translate(
            string text, EStructure st = EStructure.Statement)
        {
            return null;
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
            return Top();
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

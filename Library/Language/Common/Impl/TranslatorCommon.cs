using System.Collections.Generic;
using System.Text;
using Pyro.Exec;

namespace Pyro.Language.Impl {
    /// <inheritdoc cref="ITranslator" />
    /// <summary>
    ///     Common to all processes that translate an AST to Pi code sequences.
    /// </summary>
    public class TranslatorCommon
        : ProcessCommon
            , ITranslator {
        private readonly Stack<Continuation> _stack = new Stack<Continuation>();

        protected TranslatorCommon(IRegistry r) : base(r) {
            Reset();
        }

        protected virtual Continuation Result => _stack.Count == 0 ? null : Top();
        public int TraceLevel { get; set; }

        public virtual bool Translate(string text, out Continuation cont, EStructure st = EStructure.Program) {
            Reset();
            cont = null;
            return !string.IsNullOrEmpty(text) || Fail("Empty input");
        }

        public new void Reset() {
            base.Reset();
            _stack.Clear();
            PushNew();
        }

        public override string ToString() {
            var str = new StringBuilder();
            foreach (var cont in _stack)
                str.Append(cont);

            return str.ToString();
        }

        protected bool PushNew() {
            _stack.Push(Continuation.New(_reg));
            return true;
        }

        protected Continuation Pop() {
            return _stack.Pop();
        }

        protected Continuation Top() {
            return _stack.Peek();
        }

        protected bool Append(object obj) {
            Top().Code.Add(obj);
            return true;
        }
    }
}
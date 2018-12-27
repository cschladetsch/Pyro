using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Diver.Exec
{
    /// <summary>
    /// Processes a sequence of Continuations.
    /// </summary>
    public partial class Executor
    {
        public Stack<object> DataStack => _data;
        public Stack<IRef<Continuation>> ContextStack => _context;

        public Executor()
        {
            AddOperations();
        }

        private void AddOperations()
        {
            _actions[EOperation.Plus] = () =>
            {
                var b = ResolvePop();
                var a = ResolvePop();
                Push(a + b);
            };
            _actions[EOperation.Minus] = () =>
            {
                var a = ResolvePop();
                var b = ResolvePop();
                Push(a - b);
            };
            _actions[EOperation.Multiply] = () => Push(Pop() * Pop());
            _actions[EOperation.Divide] = () => Push(Pop() / Pop());

            _actions[EOperation.Suspend] = Suspend;
            _actions[EOperation.Resume] = Resume;
            _actions[EOperation.Replace] = Break;
            _actions[EOperation.Break] = DebugBreak;

            _actions[EOperation.Store] = StoreValue;
            _actions[EOperation.Retrieve] = GetValue;
            _actions[EOperation.Assert] = Assert;
            _actions[EOperation.Not] = () => Push(!Pop<bool>());
            _actions[EOperation.Equiv] = Equiv;
        }

        private void Equiv()
        {
            var a = Pop();
            var b = Pop();
            Push(a.Equals(b));
        }

        void Assert()
        {
            if (!Pop<bool>())
                throw new AssertionFailedException();
        }

        private dynamic ResolvePop()
        {
            return Resolve(Pop());
        }

        private void DebugBreak()
        {
            throw new DebugBreakException();
        }

        private void StoreValue()
        {
            var name = Pop<IdentBase>();
            var val = Pop();
            if (name is Label label)
            {
                Context().SetScopeObject(label.Text, val);
                return;
            }

            throw new NotImplementedException();
        }

        private void GetValue()
        {
            var label = Pop<string>();
            var fromScope = Context().FromScope(label);
            Push(fromScope);
        }

        public void Continue(IRef<Continuation> continuation)
        {
            _current = continuation;
            while (true)
            {
                Execute(_current.Value);
                _break = false;
                if (_context.Count > 0)
                    _current = _context.Pop();
                else
                    break;
            }
        }

        private void Execute(Continuation cont)
        {
            while (cont.Next(out var next))
            {
                // unbox reference types
                if (next is IRefBase refBase)
                    next = refBase.BaseValue;

                try
                {
                    Perform(next);
                    if (TraceLevel > 5)
                    {
                        if (next is EOperation op)
                        {
                            Write("-->");
                            WriteDataStack();
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteLine(e);
                    WriteLine(DebugWrite());
                    throw;
                }

                if (_break)
                    break;
            }
        }

        private void Perform(object next)
        {
            PerformPrelude(next);
            if (next is EOperation op)
            {
                if (_actions.TryGetValue(op, out var action))
                    action();
                else
                    throw new NotImplementedException($"Operation {op}");
            }
            else
            {
                var item = Resolve(next);
                if (item == null)
                    throw new NullValueException();
                _data.Push(item);
            }
        }

        private object Resolve(object next)
        {
            if (next == null)
                return null;
            if (!(next is IdentBase ident))
                return next;
            return ident.Quoted ? ident : Resolve(ident);
        }

        private object Resolve(IdentBase identBase)
        {
            if (identBase is Label label)
                return ResolveContextually(label);

            if (identBase is Pathname path)
                return ResolvePath(path);

            throw new NotImplementedException($"Cannot resolve {identBase}");
        }

        private object ResolvePath(Pathname path)
        {
            throw new NotImplementedException();
        }

        private object ResolveContextually(Label label)
        {
            var current = Context();
            var ident = label.Text;
            if (current.HasScopeObject(ident))
                return current.Scope[ident];
            foreach (var contRef in _context)
            {
                var cont = contRef.Value;
                if (cont.HasScopeObject(ident))
                    return cont.Scope[ident];
            }

            return null;
        }

        private Continuation Context()
        {
            return _current.Value;
        }

        /// <summary>
        /// Perform a continuation, then return to current context
        /// </summary>
        private void Suspend()
        {
            _context.Push(_current);
            Resume();
        }

        /// <summary>
        /// Resume the continuation that spawned the current one
        /// </summary>
        private void Resume()
        {
            _context.Push(Pop());
            Break();
        }

        /// <summary>
        /// Stop the current continuation and resume whatever is on the context stack
        /// </summary>
        private void Break()
        {
            _break = true;
        }

        private void Push(object obj)
        {
            if (obj == null)
                throw new NullValueException();
            _data.Push(obj);
        }

        public T Pop<T>()
        {
            if (_data.Count == 0)
                throw new DataStackEmptyException();
            var top = Pop();
            if (top == null)
                throw new NullValueException();
            if (top is T val)
                return val;
            if (!(top is IRef<T> data))
                throw new TypeMismatchError(typeof(T), top.GetType());
            return data.Value;
        }

        private dynamic Pop()
        {
            if (_data.Count == 0)
                throw new DataStackEmptyException();

            var pop = _data.Pop();
            return !(pop is IRefBase data) ? pop : data.BaseValue;
        }

        private bool _break;
        private Stack<object> _data = new Stack<object>();
        private IRef<Continuation> _current;
        private Stack<IRef<Continuation>> _context = new Stack<IRef<Continuation>>();
        private readonly Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();

        public void Clear()
        {
            _data = new Stack<object>();
            _context = new Stack<IRef<Continuation>>();
            _break = false;
            _current = null;
        }
    }

}

using System;
using System.Collections.Generic;

namespace Pyro.Exec
{
    /// <inheritdoc />
    /// <summary>
    /// Processes a sequence of objects in a sequence of Continuations.
    /// </summary>
    public partial class Executor
        : Reflected<Executor>
    {
        public Stack<object> DataStack { get; private set; } = new Stack<object>();
        public Stack<Continuation> ContextStack { get; private set; } = new Stack<Continuation>();
        public int NumOps { get; private set; }
        public bool Rethrows { get; set; }
        public string SourceFilename;

        private bool _break;
        private Continuation _current;
        private readonly Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();
        private IRegistry _registry => Self.Registry;

        public Executor() => AddOperations();
        public void Continue(IRef<Continuation> continuation) => Continue(continuation.Value);
        public void PushContext(Continuation continuation) => ContextStack.Push(continuation);
        public void Continue() => Continue(ContextStack.Pop());

        private Continuation Context() => _current;
        private dynamic RPop() => Resolve(Pop());
        private dynamic RPop<T>() => ResolvePop<T>();
        private dynamic ResolvePop<T>() => Resolve(Pop<T>());
        private static void DebugBreak() => throw new DebugBreakException();

        /// <summary>
        /// Stop the current continuation and resume whatever is on the context stack
        /// </summary>
        private void Break() => _break = true;

        public void Clear()
        {
            DataStack = new Stack<object>();
            ContextStack = new Stack<Continuation>();
            NumOps = 0;

            _break = false;
            _current = null;
        }

        private void Assert()
        {
            if (!Pop<bool>())
                throw new AssertionFailedException();
        }

        private void StoreValue()
        {
            var name = Pop<IdentBase>();
            var val = Pop();
            if (name is Label label)
                Context().SetScopeObject(label.Text, val);
            else
                throw new Exception($"Can't store to {name}");
        }

        private void GetValue()
        {
            var label = Pop<string>();
            var fromScope = Context().FromScope(label);
            Push(fromScope);
        }

        public void Continue(Continuation continuation)
        {
            _current = continuation;
            _break = false;

            while (true)
            {
                Execute(_current);
                _break = false;
                if (ContextStack.Count > 0)
                {
                    _current = ContextStack.Pop();
                    _current.Enter(this);
                }
                else
                    break;
            }
        }

        public bool Step()
        {
            if (_current == null)
                _current = ContextStack.Pop();

            if (_current == null)
                return false;

            if (_current.Next(out var next))
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
                            Write($"{op} -->");
                            WriteDataStack();
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(SourceFilename))
                        WriteLine($"While executing {SourceFilename}:");

                    if (TraceLevel > 10)
                    {
                        WriteLine(DebugWrite());
                        WriteLine($"Exception: {e}");
                    }

                    throw;
                }
            }

            return true;
        }

        private void Execute(Continuation cont)
        {
            ContextStack.Push(cont);
            while (Step())
                if (_break)
                    break;
        }

        public void Perform(object next)
        {
            ++NumOps;
            if (next == null)
                throw new NullValueException("Cannot Perform null");

            PerformPrelude(next);
            switch (next)
            {
            case EOperation op when _actions.TryGetValue(op, out var action):
                action();
                break;

            case EOperation op:
                throw new NotImplementedException($"Operation {op} not implemented");

            default:
                var item = Resolve(next);
                if (item == null)
                {
                    item = Resolve(next);
                    throw new UnknownIdentifierException(next);
                }
                DataStack.Push(item);
                break;
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
            switch (identBase)
            {
            case Label label:
                return _registry.GetClass(label.Text) ?? ResolveContextually(label);

            case Pathname path:
                return ResolvePath(path);
            }

            throw new CannotResolve($"{identBase}");
        }

        private static object ResolvePath(Pathname path)
        {
            throw new NotImplementedException();
        }

        private object ResolveContextually(Label label)
        {
            var current = Context();
            var ident = label.Text;
            if (current.HasScopeObject(ident))
                return current.Scope[ident];

            foreach (var cont in ContextStack)
                if (cont.HasScopeObject(ident))
                    return cont.Scope[ident];

            return Scope.TryGetValue(ident, out var obj) ? obj : null;
        }

        /// <summary>
        /// Perform a continuation, then return to current context
        /// </summary>
        private void Suspend()
        {
            ContextStack.Push(_current);
            Resume();
        }

        /// <summary>
        /// Resume the continuation that spawned the current one
        /// </summary>
        private void Resume()
        {
            var next = RPop();
            switch (next)
            {
                case ICallable call:
                    call.Invoke(_registry, DataStack);
                    break;

                case IClassBase @class:
                    Push(@class.NewInstance(DataStack));
                    break;

                default:
                    ContextStack.Push(next);
                    break;
            }

            Break();
        }

        public void Push(object obj)
        {
            if (obj == null)
                throw new NullValueException();

            DataStack.Push(obj);
        }

        public T Pop<T>()
        {
            if (DataStack.Count == 0)
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

        public dynamic Pop()
        {
            if (DataStack.Count == 0)
                throw new DataStackEmptyException();

            var pop = DataStack.Pop();
            return !(pop is IRefBase data) ? pop : data.BaseValue;
        }
    }
}


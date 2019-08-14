namespace Pyro.Exec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Flow;

    /// <inheritdoc />
    /// <summary>
    /// Processes a sequence of Continuations.
    /// </summary>
    public partial class Executor
        : Reflected<Executor>
    {
        public Stack<object> DataStack { get; private set; } = new Stack<object>();
        public Stack<Continuation> ContextStack { get; private set; } = new Stack<Continuation>();
        public int NumOps { get; private set; }
        public bool Rethrows { get; set; }
        public string SourceFilename;
        public void PushContext(Continuation continuation) => ContextStack.Push(continuation);
        public void Continue(IRef<Continuation> continuation) => Continue(continuation.Value);
        public void Continue() => Continue(ContextStack.Pop());
        public IKernel Kernel;

        private bool _break;
        private Continuation _current;
        private readonly Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();
        private IRegistry _registry => Self.Registry;

        public Executor()
        {
            Kernel = Flow.Create.Kernel();
            AddOperations();
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

        private void Execute(Continuation cont)
        {
            while (cont.Next(out var next))
            {
                // unbox pyro-reference types
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

                    if (Rethrows)
                        throw;
                }

                if (_break)
                    break;
            }
        }

        public void Perform(object next)
        {
            ++NumOps;
            if (next == null)
                throw new NullValueException();

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
            {
                if (cont.HasScopeObject(ident))
                    return cont.Scope[ident];
            }

            return Scope.TryGetValue(ident, out var obj) ? obj : null;
        }

        private Continuation Context()
        {
            return _current;
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
                    Push(@class.NewInstance());//DataStack));
                    break;

                case MethodInfo mi:
                    var obj = Pop();
                    var numArgs = mi.GetParameters().Length;
                    var args = DataStack.Take(numArgs).ToArray();
                    var ret = mi.Invoke(obj, args);
                    if (mi.ReturnType != typeof(void))
                        Push(ret);
                    break;

                default:
                    ContextStack.Push(next);
                    break;
            }

            Break();
        }

        /// <summary>
        /// Stop the current continuation and resume whatever is on the context stack
        /// </summary>
        private void Break()
        {
            _break = true;
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

        private dynamic RPop()
            => Resolve(Pop());

        private dynamic RPop<T>()
            => ResolvePop<T>();

        private dynamic ResolvePop<T>()
            => Resolve(Pop<T>());

        private static void DebugBreak()
            => throw new DebugBreakException();
    }
}


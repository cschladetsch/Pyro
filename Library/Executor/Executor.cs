namespace Pyro.Exec
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using Flow;

    /// <inheritdoc />
    /// <summary>
    /// Processes a sequence of Continuations.
    /// </summary>
    public partial class Executor
        : Reflected<Executor>
    {
        public Stack<object> DataStack { get; private set; } = new Stack<object>();
        public List<Continuation> ContextStack { get; private set; } = new List<Continuation>();
        public int NumOps { get; private set; }
        public bool Rethrows { get; set; }
        public string SourceFilename;

        private bool _break;
        private Continuation _current;
        private readonly Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();
        private IRegistry _registry => Self.Registry;
        private int _nextContext;

        public Executor()
        {
            Kernel = Flow.Create.Kernel();
            Rethrows = true;
            Verbosity = 0;
            Verbosity = 100;
            AddOperations();
        }

        public void PushContext(Continuation continuation)
        {
            ContextStack.Add(continuation);
            Info($"PushContext: {ContextStack.Count}");
            _nextContext = ContextStack.Count - 1;
        }

        public void Continue(IRef<Continuation> continuation)
            => Continue(continuation.Value);

        public void Continue()
        {
            // TODO ???
            //_nextContext = ContextStack.Count - 1;
            Continue(PopContext());
        }

        public void Continue(Continuation continuation)
        {
            Prepare(continuation);

            while (true)
            {
                Execute(_current);
                _break = false;

                _current = PopContext();
                if (_current == null)
                    break;
            }
        }

        public void Prepare(Continuation cont)
        {
            _current = cont;
            _break = false;
            cont.Enter(this);
        }

        private void Execute(Continuation cont)
        {
            _current = cont;
            while (Next())
            {
                if (_break)
                {
                    _break = false;
                    _current = PopContext();
                }
            }
        }

        public bool Next()
        {
            Kernel.Step();

            bool GetCurrent()
            {
                if (_current != null)
                    return true;

                _current = PopContext();
                return _current != null;
            }

            if (!GetCurrent())
                return false;

            if (!_current.Next(out var next))
            {
                //PopContext();
                return false;
            }

            if (!GetCurrent())
                return false;

            // unbox pyro-reference types
            if (next is IRefBase refBase)
                next = refBase.BaseValue;

            try
            {
                Perform(next);

                //if (Verbosity > 10)
                //    Write($"{next} ");
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(SourceFilename))
                    WriteLine($"While executing {SourceFilename}:");

                if (Verbosity > 10)
                {
                    WriteLine(DebugWrite());
                    WriteLine($"Exception: {e}");
                }

                if (Rethrows)
                    throw;

                return false;
            }

            return true;
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

        private bool TryResolve(IdentBase identBase, out object found)
        {
            found = null;
            switch (identBase)
            {
            case Label label:
                found = _registry.GetClass(label.Text) ?? ResolveContextually(label);
                return found != null;

            case Pathname path:
                found = ResolvePath(path);
                return found != null;
            }

            return false;
        }

        private object Resolve(IdentBase identBase)
        {
            if (TryResolve(identBase, out var res))
                return res;

            TryResolve(identBase, out var res2);

            throw new CannotResolve($"{identBase}");
        }

        private static object ResolvePath(Pathname path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to resolve a name by looking at the context stack
        /// </summary>
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
            => _current;

        /// <summary>
        /// Perform a continuation, then return to current context
        /// </summary>
        private new void Suspend()
        {
            PushContext(_current);
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
                    var args = new object[numArgs];
                    for (var n = 0; n < numArgs; ++n)
                        args[n] = DataStack.Pop();
                    var ret = mi.Invoke(obj, args);
                    if (mi.ReturnType != typeof(void))
                        Push(ret);
                    break;

                default:
                    //PushContext(next);
                    _current = next;
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

        private Continuation PopContext()
        {
            for (var n = _nextContext; n >= 0; ++n)
            {
                if (n >= ContextStack.Count)
                    break;

                var c = ContextStack[n];
                if (c.Active && c.Running)
                {
                    ContextStack.RemoveAt(n);
                    _nextContext--;
                    c.Enter(this);
                    return c;
                }
            }

            return null;
        }

        /// <summary>
        /// Stop the current continuation and resume whatever is on the
        /// context stack.
        /// </summary>
        private void Break()
            => _break = true;

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


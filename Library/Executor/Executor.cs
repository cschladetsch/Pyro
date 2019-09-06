using System.ComponentModel;
using System.Threading;

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
            //Rethrows = true;
            Verbosity = 0;
            Verbosity = 100;
            AddOperations();
        }

        public void PushContext(Continuation continuation)
        {
            ContextStack.Add(continuation);
            _current = null;
            //if (continuation.Ip < continuation.Code.Count)
            //    continuation.Running = true;
            //_nextContext = ContextStack.Count - 1;
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
            PushContext(continuation);

            while (true)
            {
                Execute(_current);
                _break = false;

                _current = PopContext();
                if (_current == null)
                    break;
            }
        }

        //public void Prepare(Continuation cont)
        //{
        //    _current = cont;
        //    _break = false;
        //    cont.Enter(this);
        //}

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

            bool IsRunning(Continuation cont)
                => cont != null && cont.IsRunning();

            while (!IsRunning(_current))
            {
                _current = PopContext();
                if (_current == null)
                    return false;
            }

            var end = !_current.Next(out var next);
            if (next is IRefBase refBase)
                next = refBase.BaseValue;

            try
            {
                Perform(next);
                if (end)
                    _current.Complete();
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
                if (!TryResolve(next, out var eval))
                    throw new UnknownIdentifierException(next);

                DataStack.Push(eval);
                break;
            }
        }

        private bool TryResolve(object obj, out object found)
        {
            found = null;
            if (obj == null)
                return false;

            if (!(obj is IdentBase ident))
            {
                found = obj;
                return true;
            }

            if (!ident.Quoted)
                return TryResolve(ident, out found);

            found = obj;
            return true;
        }

        private bool TryResolve(IdentBase identBase, out object found)
        {
            found = null;
            switch (identBase)
            {
            case Label label:
                found = _registry.GetClass(label.Text);
                if (found != null)
                    return true;

                // TODO: search Sytem types like System.Int32 etc

                return TryResolve(label, out found);

            case Pathname path:
                return TryResolvePath(path, out found);
            }

            return false;
        }

        private bool TryResolvePath(Pathname path, out object found)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to resolve a name by looking at the context stack
        /// </summary>
        private bool ResolveContextually(Label label, out object obj)
        {
            obj = null;
            var ident = label.Text;

            var current = Context();
            if (current.Scope.TryGetValue(ident, out obj))
                return true;

            foreach (var cont in ContextStack)
                if (cont.Scope.TryGetValue(ident, out obj))
                    return true;

            return Scope.TryGetValue(ident, out obj);
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
            if (!RPop(out var next))
            {
                Break();
                return;
            }
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
                    PushContext(next);
                    _current = null;
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
            for (var n = ContextStack.Count - 1; n >= 0; --n)
            {
                var cont = ContextStack[n];
                if (!cont.Active || !cont.Running)
                    continue;

                ContextStack.RemoveAt(n);
                return _current = cont.Start(this);
            }

            return null;
        }

        /// <summary>
        /// Stop the current continuation and resume whatever is on the
        /// context stack.
        /// </summary>
        private void Break()
        {
            _break = true;
            _current = null;
        }

        private T RPop<T>()
        {
            if (RPop<T>(out var val))
                return val;
            throw new DataStackEmptyException();
        }

        /// <summary>
        /// Get top of stack as a value of type T, or as a name of a value of type T
        /// </summary>
        private bool RPop<T>(out T val)
        {
            val = default(T);
            var pop = (object) Pop();
            if (!TryResolve(pop, out var found))
                return false;

            val = (T) found;
            return true;
        }

        private bool RPop(out dynamic val)
            => TryResolve(Pop(), out val);

        private static void DebugBreak()
            => throw new DebugBreakException();
    }
}


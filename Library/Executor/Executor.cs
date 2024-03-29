using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pyro.Exec {
    /// <summary>
    ///     Processes a sequence of Continuations.
    /// </summary>
    public partial class Executor
        : Reflected<Executor> {
        public delegate void ContextStackChangedHandler(Executor executor, List<Continuation> context);

        public delegate void ContinuationChangedHandler(Executor executor, Continuation previous, Continuation current);

        public delegate void DataStackChangedHandler(Executor executor, Stack<object> context);

        private readonly Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();
        private bool _break;

        public string SourceFilename = "<script>";

        public Executor() {
            Kernel = Flow.Create.Kernel();
            Rethrows = true;
            Verbosity = 5;
            AddOperations();
        }

        public Stack<object> DataStack { get; } = new Stack<object>();

        public List<Continuation> ContextStack { get; } = new List<Continuation>();

        public bool Rethrows { get; set; }

        public Continuation Current { get; private set; }

        private int NumOps { get; set; }
        private IRegistry _registry => Self.Registry;
        public event ContextStackChangedHandler OnContextStackChanged;
        public event DataStackChangedHandler OnDataStackChanged;
        public event ContinuationChangedHandler OnContinuationChanged;

        private void SetCurrent(Continuation continuation) {
            Current?.FireOnLeave();
            FireContinuationChanged(Current, continuation);
            Current = continuation;
        }

        private void FireContextStackChanged() {
            OnContextStackChanged?.Invoke(this, ContextStack);
        }

        private void FireDataStackChanged() {
            OnDataStackChanged?.Invoke(this, DataStack);
        }

        private void FireContinuationChanged(Continuation previous, Continuation current) {
            OnContinuationChanged?.Invoke(this, previous, current);
            Current = current;
        }

        public void Continue(IRef<Continuation> continuation) {
            Continue(continuation.Value);
        }

        public void Continue() {
            Continue(PopContext());
        }

        public void Continue(Continuation continuation) {
            PushContext(continuation);

            while (true) {
                if (ContextStack.Count == 0) {
                    break;
                }

                SetCurrent(PopContext());

                _break = false;
                while (Next())
                    if (_break) {
                        break;
                    }
            }
        }

        public bool Next() {
            if (!StepCurrent(out var next)) {
                return false;
            }

            try {
                Perform(next);
            } catch (Exception e) {
                HandleException(e);
                return false;
            }

            return true;
        }

        private void HandleException(Exception e) {
            if (!string.IsNullOrEmpty(SourceFilename)) {
                WriteLine($"While executing {SourceFilename}:");
            }

            if (Verbosity > 5) {
                WriteLine(DebugWrite());
                WriteLine($"Exception: {e}");
            }

            if (Rethrows) {
                throw e;
            }
        }

        private bool StepCurrent(out object next) {
            Kernel.Step();

            GetCurrent();

            if (!Current.Next(out next)) {
                Break();
                return false;
            }

            if (next is IRefBase refBase) {
                next = refBase.BaseValue;
            }

            return true;
        }

        private void GetCurrent() {
            if (Current != null) {
                return;
            }

            SetCurrent(PopContext());
            if (Current == null) {
                throw new Exception("No current continuation");
            }
        }

        public void Prev() {
            throw new NotImplementedException();
        }

        public void Perform(object next) {
            ++NumOps;
            if (next == null) {
                throw new NullValueException();
            }

            if (Verbosity > 4) {
                PerformPrelude(next);
            }

            switch (next) {
                case EOperation op when _actions.TryGetValue(op, out var action):
                    action();
                    break;

                case EOperation op:
                    throw new NotImplementedException($"Operation {op} not implemented");

                default:
                    if (!TryResolve(next, out var eval)) {
                        throw new UnknownIdentifierException(next);
                    }

                    DataStackPush(eval);
                    break;
            }
        }

        private bool TryResolve(object obj, out object found) {
            found = null;
            if (obj == null) {
                return false;
            }

            if (!(obj is IdentBase ident)) {
                found = obj;
                return true;
            }

            if (!ident.Quoted) {
                return TryResolve(ident, out found);
            }

            found = obj;
            return true;
        }

        private bool TryResolve(IdentBase identBase, out object found) {
            found = null;
            switch (identBase) {
                case Label label:
                    // TODO: search System types like System.Int32 etc
                    found = _registry.GetClass(label.Text);
                    if (found != null) {
                        return true;
                    }

                    if (TryResolveContextually(label, out found)) {
                        return true;
                    }

                    if (Scope.TryGetValue(label.Text, out found)) {
                        return true;
                    }

                    return false;

                case Pathname path:
                    return TryResolvePath(path, out found);
            }

            return false;
        }

        private bool TryResolvePath(Pathname path, out object found) {
            throw new NotImplementedException();
        }

        private bool TryResolveContextually(Label label, out object obj) {
            obj = null;
            var ident = label.Text;

            var current = Current;
            if (current.Scope.TryGetValue(ident, out obj)) {
                return true;
            }

            foreach (var cont in ContextStack)
                if (cont.Scope.TryGetValue(ident, out obj)) {
                    return true;
                }

            return Scope.TryGetValue(ident, out obj);
        }

        /// <summary>
        ///     Perform a continuation, then return to current context
        /// </summary>
        private new void Suspend() {
            PushContext(Current);
            Resume();
        }

        /// <summary>
        ///     Resume the continuation that spawned the current one
        /// </summary>
        private new void Resume() {
            if (!ResolvePop(out var next)) {
                throw new DataStackEmptyException("Cannot resume: empty data stack");
            }

            switch (next) {
                case ICallable call:
                    call.Invoke(_registry, DataStack);
                    FireDataStackChanged();
                    break;

                case IClassBase @class:
                    Push(@class.NewInstance());
                    break;

                case MethodInfo mi:
                    ResumeMethod(mi);
                    break;

                default:
                    ResumeContinuation(next);
                    break;
            }

            Break();
        }

        private void ResumeContinuation(dynamic next) {
            if (!(next is Continuation cont)) {
                throw new Exception("Cannot resume type " + next.GetType());
            }

            PushContext(cont);
            cont.Enter(this);
        }

        private void ResumeMethod(MethodInfo mi) {
            var numArgs = mi.GetParameters().Length;
            if (DataStack.Count < numArgs + 1) {
                var servant = DataStack.Count > 0 ? DataStack.Peek() : "null";
                throw new NotEnoughArgumentsException($"{servant}.{mi.Name} expects {numArgs} args");
            }

            var obj = Pop();
            var args = new object[numArgs];
            for (var n = 0; n < numArgs; ++n) args[numArgs - n - 1] = Pop();

            var ret = mi.Invoke(obj, args);
            if (mi.ReturnType != typeof(void)) {
                Push(ret);
            }
        }

        public void Push(object obj) {
            if (obj == null) {
                throw new NullValueException();
            }

            DataStackPush(obj);
        }

        public T Pop<T>() {
            if (DataStack.Count == 0) {
                throw new DataStackEmptyException();
            }

            var top = Pop();
            if (top == null) {
                throw new NullValueException();
            }

            if (top is T val) {
                return val;
            }

            if (!(top is IRef<T> data)) {
                throw new TypeMismatchError(typeof(T), top.GetType());
            }

            return data.Value;
        }

        public dynamic Pop() {
            if (DataStack.Count == 0) {
                throw new DataStackEmptyException();
            }

            var pop = DataStackPop();
            return !(pop is IRefBase data) ? pop : data.BaseValue;
        }

        private object DataStackPop() {
            var top = DataStack.Pop();
            FireDataStackChanged();
            return top;
        }

        private void DataStackPush(object obj) {
            DataStack.Push(obj);
            FireDataStackChanged();
        }

        public void PushContext(Continuation continuation) {
            if (continuation == null) {
                throw new ArgumentNullException("Cannot push null context " + nameof(continuation));
            }

            ContextStack.Add(continuation);
            FireContextStackChanged();
        }

        private Continuation PopContext() {
            var contextStackCount = ContextStack.Count;
            if (contextStackCount == 0) {
                throw new ContextStackEmptyException();
            }

            var last = contextStackCount - 1;
            var cont = ContextStack[last];
            if (cont == null) {
                throw new NullValueException("Unexpected null continuation");
            }

            ContextStack.RemoveAt(last);

            FireContextStackChanged();
            return cont;
        }

        /// <summary>
        ///     Stop the current continuation and resume whatever is on the
        ///     context stack.
        /// </summary>
        private void Break() {
            _break = true;
        }

        private T ResolvePop<T>() {
            if (ResolvePop<T>(out var val)) {
                return val;
            }

            throw new DataStackEmptyException();
        }

        private bool ResolvePop<T>(out T val) {
            val = default;
            var pop = (object)Pop();
            if (!TryResolve(pop, out var found)) {
                return false;
            }

            val = (T)found;
            return true;
        }

        private bool ResolvePop(out dynamic val) {
            return TryResolve(Pop(), out val);
        }

        private static void DebugBreak() {
            throw new DebugBreakException();
        }
    }
}
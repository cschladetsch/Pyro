using System;
using System.Collections.Generic;

namespace Pyro.Impl {
    public class VoidMethod<T>
        : CallableBase
            , ICallable<T>
        where T : class {
        private readonly Action<T> _fun;

        public VoidMethod(Action<T> fun)
            : base(fun) {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var obj = stack.Pop();
            _fun(reg.Get<T>(obj));
        }
    }

    public class VoidMethod<T, A0>
        : CallableBase
            , ICallable<T>
        where T : class {
        private readonly Action<T, A0> _fun;

        public VoidMethod(Action<T, A0> fun)
            : base(fun) {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var obj = stack.Pop();
            var a = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A0>(a));
        }
    }


    /// <summary>
    ///     A callable thing that doesn't return anything and takes two arguments.
    /// </summary>
    public class VoidMethod<T, A0, A1>
        : CallableBase
            , ICallable<T>
        where T : class {
        private readonly Action<T, A0, A1> _fun;

        public VoidMethod(Action<T, A0, A1> fun)
            : base(fun) {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var obj = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1));
        }
    }

    public class VoidMethod<T, A0, A1, A2>
        : CallableBase
            , ICallable<T>
        where T : class {
        private readonly Action<T, A0, A1, A2> _fun;

        public VoidMethod(Action<T, A0, A1, A2> fun)
            : base(fun) {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var obj = stack.Pop();
            var a2 = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1), reg.Get<A2>(a2));
        }
    }

    public class VoidMethod<T, A0, A1, A2, A3>
        : CallableBase
            , ICallable<T>
        where T : class {
        private readonly Action<T, A0, A1, A2, A3> _fun;

        public VoidMethod(Action<T, A0, A1, A2, A3> fun)
            : base(fun) {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var obj = stack.Pop();
            var a3 = stack.Pop();
            var a2 = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1), reg.Get<A2>(a2), reg.Get<A3>(a3));
        }
    }
}
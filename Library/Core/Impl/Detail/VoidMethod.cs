using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    public class VoidMethod<T, A, B, C, D>
        : CallableBase
        , ICallable<T>
            where T : class
    {
        private readonly Action<T, A, B, C, D> _fun;

        public VoidMethod(Action<T, A, B, C, D> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a0 = stack.Pop();
            var a1 = stack.Pop();
            var a2 = stack.Pop();
            var a3 = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a0), reg.Get<B>(a1), reg.Get<C>(a2), reg.Get<D>(a3));
        }
    }


    public class VoidMethod<T, A, B, C>
        : CallableBase
        , ICallable<T>
            where T : class
    {
        private readonly Action<T, A, B, C> _fun;

        public VoidMethod(Action<T, A, B, C> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a0 = stack.Pop();
            var a1 = stack.Pop();
            var a2 = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a0), reg.Get<B>(a1), reg.Get<C>(a2));
        }
    }

    /// <summary>
    /// A callable thing that doesn't return anything and takes two arguments.
    /// </summary>
    public class VoidMethod<T, A, B>
        : CallableBase
        , ICallable<T>
            where T : class
    {
        private readonly Action<T, A, B> _fun;

        public VoidMethod(Action<T, A, B> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            var b = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a), reg.Get<B>(b));
        }
    }

    public class VoidMethod<T, A>
        : CallableBase
        , ICallable<T>
            where T : class
    {
        private readonly Action<T, A> _fun;

        public VoidMethod(Action<T, A> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a));
        }
    }

    public class VoidMethod<T>
        : CallableBase
        , ICallable<T>
            where T : class
    {
        private readonly Action<T> _fun;

        public VoidMethod(Action<T> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            _fun(reg.Get<T>(obj));
        }
    }
}

using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class Callable<T, A, B, R>
        : CallableBase, ICallable<T>
            where T : class
    {
        public Callable(Func<T, A, B, R> fun) : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            var b = stack.Pop();
            var r = _fun(reg.Get<T>(obj), reg.Get<A>(a), reg.Get<B>(b));
            if (r != null)
                stack.Push(r);
        }

        private readonly Func<T, A, B, R> _fun;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class Callable<T, A, R>
        : CallableBase, ICallable<T>
            where T : class
    {
        private readonly Func<T, A, R> _fun;

        public Callable(Func<T, A, R> fun) : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            var r = _fun(reg.Get<T>(obj), reg.Get<A>(a));
            if (r != null)
                stack.Push(r);
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class Callable<T, R>
        : CallableBase, ICallable<T>
            where T : class
    {
        private readonly Func<T, R> _fun;

        public Callable(Func<T, R> fun) : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            var r = _fun(reg.Get<T>(obj));
            if (r != null)
                stack.Push(r);
        }
    }
}


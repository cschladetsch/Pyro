using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <summary>
    /// Method with no arguments.
    /// </summary>
    public class Method<T, R>
        : CallableBase, ICallable<T>
        where T : class
    {
        private readonly Func<T, R> _fun;

        public Method(Func<T, R> fun)
            : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj)));
        }
    }

    /// <summary>
    /// Method with one argument.
    /// </summary>
    public class Method<T, A0, R>
        : CallableBase, ICallable<T>
        where T : class
    {
        private readonly Func<T, A0, R> _fun;

        public Method(Func<T, A0, R> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a0 = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj), reg.Get<A0>(a0)));
        }
    }

    /// <summary>
    /// Method with two arguments.
    /// </summary>
    public class Method<T, A0, A1, R>
        : CallableBase
            , ICallable<T>
        where T : class
    {
        private readonly Func<T, A0, A1, R> _fun;

        public Method(Func<T, A0, A1, R> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1)));
        }
    }

    /// <summary>
    /// Method with three arguments.
    /// </summary>
    public class Method<T, A0, A1, A2, R>
        : CallableBase
        , ICallable<T>
        where T : class
    {
        private readonly Func<T, A0, A1, A2, R> _fun;

        public Method(Func<T, A0, A1, A2, R> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a2 = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1), reg.Get<A2>(a2)));
        }
    }
}


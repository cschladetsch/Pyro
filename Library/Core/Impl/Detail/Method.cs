using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <summary>
    /// A callable method with two arguments.
    /// </summary>
    /// <typeparam name="T">The type of object to invoke the method on.</typeparam>
    /// <typeparam name="R">Return type.</typeparam>
    /// <typeparam name="A0">First argument type.</typeparam>
    /// <typeparam name="A1">Second argument type.</typeparam>
    public class Method<T, A0, A1, R>
        : CallableBase
        , ICallable<T>
        where T : class
    {
        private readonly Func<T, A0, A1, R> _fun;

        public Method(Func<T, A0, A1, R> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a1 = stack.Pop();
            var a0 = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj), reg.Get<A0>(a0), reg.Get<A1>(a1)));
        }
    }

    /// <summary>
    /// Method with one argument.
    /// </summary>
    /// <typeparam name="T">The type of object to invoke the method on.</typeparam>
    /// <typeparam name="R">Return type.</typeparam>
    /// <typeparam name="A0">First argument type.</typeparam>
    public class Method<T, A0, R>
        : CallableBase, ICallable<T>
        where T : class
    {
        private readonly Func<T, A0, R> _fun;

        public Method(Func<T, A0, R> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a0 = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj), reg.Get<A0>(a0)));
        }
    }

    /// <summary>
    /// Method with no arguments.
    /// </summary>
    /// <typeparam name="T">The type of object to invoke the method on.</typeparam>
    /// <typeparam name="R">Return type.</typeparam>
    public class Method<T, R>
        : CallableBase, ICallable<T>
        where T : class
    {
        private readonly Func<T, R> _fun;

        public Method(Func<T, R> fun) : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            stack.Push(_fun(reg.Get<T>(obj)));
        }
    }
}


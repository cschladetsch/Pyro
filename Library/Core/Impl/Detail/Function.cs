using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <inheritdoc cref="CallableBase" />
    /// <summary>
    /// Method with no arguments.
    /// </summary>
    /// <typeparam name="R">Return type.</typeparam>
    public class Function<R>
        : CallableBase
    {
        private readonly Func<R> _fun;

        public Function(Func<R> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
            => stack.Push(_fun());
    }

    /// <summary>
    /// Function with one argument.
    /// </summary>
    /// <typeparam name="R">Return type.</typeparam>
    /// <typeparam name="A0">First argument type.</typeparam>
    public class Function<A0, R>
        : CallableBase
    {
        private readonly Func<A0, R> _fun;

        public Function(Func<A0, R> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var a = stack.Pop();
            stack.Push(_fun(reg.Get<A0>(a)));
        }
    }

    /// <summary>
    /// A function with two arguments.
    /// </summary>
    /// <typeparam name="R">Return type.</typeparam>
    /// <typeparam name="A0">First argument type.</typeparam>
    /// <typeparam name="A1">Second argument type.</typeparam>
    public class Function<A0, A1, R>
        : CallableBase
    {
        private readonly Func<A0, A1, R> _fun;

        public Function(Func<A0, A1, R> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var b = stack.Pop();
            var a = stack.Pop();
            stack.Push(_fun(reg.Get<A0>(a), reg.Get<A1>(b)));
        }
    }
}


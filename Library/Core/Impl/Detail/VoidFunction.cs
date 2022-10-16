using System;
using System.Collections.Generic;

namespace Pyro.Impl {
    /// <inheritdoc cref="CallableBase" />
    /// <summary>
    /// Function with no arguments, returning void.
    /// </summary>
    public class VoidFunction
        : CallableBase {
        private readonly Action _fun;

        public VoidFunction(Action fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
            => _fun();
    }

    /// <inheritdoc cref="CallableBase" />
    /// <summary>
    /// Function with one argument, returning void.
    /// </summary>
    /// <typeparam name="A0">First argument type.</typeparam>
    public class VoidFunction<A0>
        : CallableBase {
        private readonly Action<A0> _fun;

        public VoidFunction(Action<A0> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var a = stack.Pop();
            _fun(reg.Get<A0>(a));
        }
    }

    /// <inheritdoc cref="CallableBase" />
    /// <summary>
    /// A function with two arguments, returning void.
    /// </summary>
    /// <typeparam name="A0">First argument type.</typeparam>
    /// <typeparam name="A1">Second argument type.</typeparam>
    public class VoidFunction<A0, A1>
        : CallableBase {
        private readonly Action<A0, A1> _fun;

        public VoidFunction(Action<A0, A1> fun) : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack) {
            var b = stack.Pop();
            var a = stack.Pop();
            _fun(reg.Get<A0>(a), reg.Get<A1>(b));
        }
    }
}


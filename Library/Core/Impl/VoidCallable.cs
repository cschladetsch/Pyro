using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <summary>
    /// A callable thing that doesn't return anything and takes two arguments.
    /// </summary>
    public class VoidCallable<T, A, B>
        : CallableBase, ICallable<T>
            where T : class
    {
        private readonly Action<T, A, B> _fun;

        public VoidCallable(Action<T, A, B> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            var b = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a), reg.Get<B>(b));
        }
    }

    public class VoidCallable<T, A>
        : CallableBase, ICallable<T>
            where T : class
    {
        private readonly Action<T, A> _fun;

        public VoidCallable(Action<T, A> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var a = stack.Pop();
            _fun(reg.Get<T>(obj), reg.Get<A>(a));
        }
    }

    public class VoidCallable<T>
        : CallableBase, ICallable<T>
            where T : class
    {
        private readonly Action<T> _fun;

        public VoidCallable(Action<T> fun)
            : base(fun) => _fun = fun;

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            _fun(reg.Get<T>(obj));
        }
    }
}

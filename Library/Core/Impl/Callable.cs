using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace Diver.Impl
{
    public class Callable<T, A, B, R> : CallableBase, ICallable<T> where T : class
    {
        public Callable(Func<T, A, B, R> fun) : base(fun)
        {
            _fun = fun;
        }

        public override void Invoke(IRegistry reg, Stack<object> stack)
        {
            var obj = stack.Pop();
            var b = stack.Pop();
            var a = stack.Pop();
            var r = _fun(reg.Get<T>(obj), reg.Get<A>(a), reg.Get<B>(b));
            if (r != null)
                stack.Push(r);

        }

        private readonly Func<T, A, B, R> _fun;
    }

    public class Callable<T, A, R> : CallableBase, ICallable<T> where T : class
    {
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

        private readonly Func<T, A, R> _fun;
    }

}




//var parameters = _delegate.Method.GetParameters();
//var numArgs = parameters.Length;
//var args = new List<object>();
//while (numArgs-- > 0)
//    args.Add(stack.Pop());

//var result = _delegate(args);
//if (result != null)
//    stack.Push(result);
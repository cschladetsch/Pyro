using System.Collections.Generic;

namespace Pyro
{
    public interface ICallable
    {
        void Invoke(IRegistry reg, Stack<object> stack);
    }

    public interface ICallable<T> : ICallable
    {
    }
}
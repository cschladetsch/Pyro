using System.Collections.Generic;

namespace Pryo
{
    public interface ICallable
    {
        void Invoke(IRegistry reg, Stack<object> stack);
    }

    public interface ICallable<T> : ICallable
    {
    }
}
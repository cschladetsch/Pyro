using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    /// <inheritdoc />
    /// <summary>
    /// TODO
    /// </summary>
    public class CallableBase
        : ICallable
    {
        protected Delegate _delegate;

        public CallableBase(Delegate del)
        {
            _delegate = del;
        }

        public virtual void Invoke(IRegistry reg, Stack<object> stack)
        {
            throw new NotImplementedException();
        }
    }
}

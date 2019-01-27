using System;
using System.Collections.Generic;

namespace Pryo.Impl
{
    public class CallableBase : ICallable
    {
        public CallableBase(Delegate del)
        {
            _delegate = del;
        }
        public virtual void Invoke(IRegistry reg, Stack<object> stack)
        {
            throw new NotImplementedException();
        }

        protected Delegate _delegate;
    }
}
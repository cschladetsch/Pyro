namespace Pyro {
    using System.Collections.Generic;

    /// <summary>
    /// Is able to be called given context of a registry and a data stack
    /// </summary>
    public interface ICallable {
        void Invoke(IRegistry reg, Stack<object> stack);
    }

    public interface ICallable<T>
        : ICallable {
    }
}


using System;
using Pyro.Impl;

namespace Pyro
{
    /// <summary>
    /// Top-level factory for Core
    /// </summary>
    public static class Create
    {
        public static IFactory Factory = new Impl.Factory();
        public static IRegistry Registry() => Factory.NewRegistry();
        public static ITree Tree() => Factory.NewTree();
        public static IPathname Pathname(string path) => new Pathname(path);

        public static ICallable Function<R>(Func<R> fun)
            => new Function<R>(fun);
        public static ICallable Callable<A, R>(Func<A, R> fun)
            where A : class => new Function<A, R>(fun);
        public static ICallable Callable<A0, A1, R>(Func<A0, A1, R> fun)
            where A0 : class where A1 : class => new Function<A0, A1, R>(fun);
    }
}


﻿using System;
using Pyro.Impl;

namespace Pyro {
    /// <summary>
    ///     Top-level factory for Core
    /// </summary>
    public static class Create {
        public static IFactory Factory = new Factory();

        public static IRegistry Registry() {
            return Factory.NewRegistry();
        }

        public static ITree Tree() {
            return Factory.NewTree();
        }

        public static IPathname Pathname(string path) {
            return new Pathname(path);
        }

        public static ICallable Function(Action act) {
            return new VoidFunction(act);
        }

        public static ICallable Function<A>(Action<A> act) {
            return new VoidFunction<A>(act);
        }

        public static ICallable Function<R>(Func<R> fun) {
            return new Function<R>(fun);
        }

        public static ICallable Method<T, A>(Action<T, A> fun)
            where T : class {
            return new VoidMethod<T, A>(fun);
        }

        public static ICallable Method<T, A0, A1>(Action<T, A0, A1> fun)
            where T : class {
            return new VoidMethod<T, A0, A1>(fun);
        }

        public static ICallable Method<T, A0, A1, A2>(Action<T, A0, A1, A2> method)
            where T : class {
            return new VoidMethod<T, A0, A1, A2>(method);
        }

        public static ICallable Method<T, A0, A1, A2, A3>(Action<T, A0, A1, A2, A3> method)
            where T : class {
            return new VoidMethod<T, A0, A1, A2, A3>(method);
        }

        public static ICallable Function<A, R>(Func<A, R> fun)
            where A : class {
            return new Function<A, R>(fun);
        }

        public static ICallable Function<A0, A1, R>(Func<A0, A1, R> fun)
            where A0 : class where A1 : class {
            return new Function<A0, A1, R>(fun);
        }
    }
}
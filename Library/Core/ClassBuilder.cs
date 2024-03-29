﻿using System;
using System.Text;
using Pyro.Impl;

namespace Pyro {
    /// <summary>
    ///     Make a new class that can added to a Registry.
    ///     This isn't always necessary, unless there are overloaded
    ///     methods in the class.
    /// </summary>
    public class ClassBuilder<T>
        where T : class {
        private readonly IRegistry _registry;
        public AddMethod Methods;
        public AddProperty Properties;

        public ClassBuilder(IRegistry reg) {
            _registry = reg;
            Class = new Class<T>(reg);
            Methods = new AddMethod(this);
        }

        public ClassBuilder(IRegistry reg, Action<IRegistry, StringBuilder, T> toText) {
            _registry = reg;
            Class = new Class<T>(reg, toText);
            Methods = new AddMethod(this);
        }

        public IClass<T> Class { get; }

        public class AdderBase {
            protected ClassBuilder<T> _builder;

            public AdderBase(ClassBuilder<T> builder) {
                _builder = builder;
            }

            public IClass<T> Class => _builder.Class;

            public TA Get<TA>(object obj) {
                return _builder._registry.Get<TA>(obj);
            }
        }

        public class AddMethod : AdderBase {
            public AddMethod(ClassBuilder<T> cb)
                : base(cb) {
            }

            public AddMethod Add(string name, Action<T> fun) {
                _builder.Class.AddCallable(name, new VoidMethod<T>(fun));
                return this;
            }

            public AddMethod Add<A>(string name, Action<T, A> fun) {
                _builder.Class.AddCallable(name, new VoidMethod<T, A>(fun));
                return this;
            }

            public AddMethod Add<A, B>(string name, Action<T, A, B> fun) {
                _builder.Class.AddCallable(name, new VoidMethod<T, A, B>(fun));
                return this;
            }

            public AddMethod Add<A0, A1, A2, A3>(string name, Action<T, A0, A1, A2, A3> method) {
                _builder.Class.AddCallable(name, new VoidMethod<T, A0, A1, A2, A3>(method));
                return this;
            }

            public AddMethod Add<A, B, R>(string name, Func<T, A, B, R> fun) {
                _builder.Class.AddCallable(name, new Method<T, A, B, R>(fun));
                return this;
            }

            public AddMethod Add<A, R>(string name, Func<T, A, R> fun) {
                _builder.Class.AddCallable(name, new Method<T, A, R>(fun));
                return this;
            }

            public AddMethod Add<R>(string name, Func<T, R> fun) {
                _builder.Class.AddCallable(name, new Method<T, R>(fun));
                return this;
            }
        }

        public class AddProperty {
            // TODO
        }
    }
}
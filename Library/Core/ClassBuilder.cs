﻿using System;
using Diver.Impl;

namespace Diver
{
    /// <summary>
    /// Make a new class that can added to a Registry. This isn't always necessary, unless there are overloaded methods in the class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassBuilder<T> where T : class//, new()
    {
        public ClassBuilder(IRegistry reg)
        {
            _registry = reg;
            _class = new Impl.Class<T>(reg);
            Methods = new AddMethod(this);
        }

        public class AdderBase
        {
            protected ClassBuilder<T> _builder;

            public AdderBase(ClassBuilder<T> builder)
            {
                _builder = builder;
            }

            public A Get<A>(object obj)
            {
                return _builder._registry.Get<A>(obj);
            }

            public IClass<T> Class => _builder._class;
        }

        public class AddMethod : AdderBase
        {
            public AddMethod(ClassBuilder<T> cb) : base(cb)
            {
            }

            public AddMethod Add<A,B,R>(string name, Func<T,A,B,R> fun)
            {
                _builder._class.AddCallable(name, new Callable<T,A,B,R>(fun));
                return this;
            }

            public AddMethod Add<A, R>(string name, Func<T, A, R> fun)
            {
                _builder._class.AddCallable(name, new Callable<T,A,R>(fun));
                return this;
            }
        }

        public class AddProperty
        {
        }

        public IClass<T> Class => _class;

        public AddMethod Methods;
        public AddProperty Property;

        private readonly IClass<T> _class;
        private readonly IRegistry _registry;
    }
}
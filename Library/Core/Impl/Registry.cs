﻿using System;
using System.Collections.Generic;

namespace Diver.Impl
{
    public class Registry : IRegistry
    {
        public Guid Guid { get; }

        public Registry()
        {
            BuiltinTypes.Builtins.Register(this);
        }

        public bool Register(IClassBase @class)
        {
            _classes.Add(@class.Type, @class);
            return true;
        }

        public IRefBase GetRef(Id id)
        {
            return _instances[id];
        }

        public T Get<T>(object obj)
        {
            switch (obj)
            {
                case T _:
                    return (T)obj;
                case IRefBase rb:
                    return GetRef<T>(rb.Id).Value;
            }

            throw new TypeMismatchError(typeof(T), obj?.GetType());
        }

        public IRefBase Add(object value)
        {
            var klass = GetClass(value?.GetType());
            return klass == null ? RefBase.None : AddNew(klass, value);
        }

        public IClass<T> GetClass<T>()
        {
            var type = typeof(T);
            var klass = FindClass(type);
            if (klass != null)
                return klass as IClass<T>;
            var typed = new Class<T>(this);
            _classes[type] = typed;
            return typed;
        }

        public IRef<T> GetRef<T>(Id id)
        {
            return _instances[id].BaseValue as IRef<T>;
        }

        public IRef<T> Add<T>(T value)
        {
            var klass = GetClass<T>();
            if (klass == null)
                throw new CouldNotMakeClass(value.GetType());
            return AddNew<T>(klass, value);
        }

        public IRef<T> Add<T>()
        {
            var klass = GetClass<T>();
            if (klass == null)
                throw new CouldNotMakeClass(typeof(T));
            return AddNew<T>(klass);
        }

        public IConstRefBase AddConst(object value)
        {
            var type = value?.GetType();
            var classBase = GetClass(type);
            if (classBase == null)
                throw new CouldNotMakeClass(value?.GetType());
            return classBase.Create(this, NextId(), value);
        }

        public IConstRef<T> AddConst<T>(T val)
        {
            throw new NotImplementedException();
        }

        public IConstRef<T> AddConst<T>()
        {
            throw new NotImplementedException();
        }

        public IClassBase GetClass(Type type)
        {
            if (type == null)
                type = typeof(void);
            var klass = FindClass(type);
            if (klass != null)
                return klass;
            return _classes[type] = new ClassBase(this, type);
        }

        private IClassBase FindClass(Type type)
        {
            return _classes.TryGetValue(type, out var klass) ? klass : null;
        }

        private IRef<T> AddNew<T>(IClassBase classBase)
        {
            var id = NextId();
            classBase.Create(this, id, out var refBase);
            _instances.Add(id, refBase);
            return refBase as IRef<T>;
        }

        private IRef<T> AddNew<T>(IClassBase classBase, T value)
        {
            var val = AddNew<T>(classBase);
            val.Value = value;
            return val;
        }

        private IRefBase AddNew(IClassBase classBase, object value)
        {
            var id = NextId();
            var refBase = classBase.Create(this, id, value);
            _instances.Add(id, refBase);
            return refBase;
        }

        private Id NextId()
        {
            return new Id(++_nextId);
        }

        private int _nextId;
        private readonly Dictionary<Type, IClassBase> _classes = new Dictionary<Type, IClassBase>();
        private readonly Dictionary<Id, IRefBase> _instances = new Dictionary<Id, IRefBase>();
    }
}

using System;
using System.Collections.Generic;

namespace Diver.Impl
{
    public class Registry : IRegistry
    {
        public Guid Guid { get; }

        public IRefBase Get(Id id)
        {
            return _instances[id];
        }

        public IRefBase Add(object value)
        {
            var klass = GetClass(value?.GetType());
            return klass == null ? RefBase.None : AddNew(klass, value);
        }

        private IClass<T> GetClass<T>()
        {
            var type = typeof(T);
            var klass = FindClass(type);
            if (klass != null)
                return klass as IClass<T>;
            var typed = new Class<T>(this, type);
            _classes[type] = typed;
            return typed;
        }

        public IRef<T> Get<T>(Id id)
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

        private IClassBase GetClass(Type type)
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

        private IRef<T> AddNew<T>(IClassBase classBase, T value)
        {
            var id = NextId();
            classBase.Create(this, id, out var refBase);
            _instances.Add(id, refBase);
            if (!(refBase is IRef<T> typed))
                return null;
            typed.Value = value;
            return typed;

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

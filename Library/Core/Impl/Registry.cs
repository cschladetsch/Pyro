using System;
using System.Collections.Generic;

namespace Diver.Impl
{
    public class Registry : IRegistry
    {
        public Guid Guid { get; }

        public object Get(Id id)
        {
            return _instances[id].BaseValue;
        }

        public void Set(Id id, object value)
        {
            _instances[id].Set(value);
        }

        public void Set<T>(Id id, T value)
        {
            _instances[id].Set(value);
        }

        public Id Add(object value)
        {
            var klass = GetClass(value?.GetType());
            if (klass == null)
                return Id.None;

            var id = NextId();
            _instances[id] = klass.Create(this, id, value);
            return id;
        }

        private IClassBase GetClass(Type type)
        {
            if (type == null)
                return null;
            return null;
        }

        public IRef<T> Get<T>(Id id)
        {
            var obj = _instances[id].BaseValue;
            return obj as IRef<T>;
        }

        public IRef<T> Add<T>(T value) //where T : class, new()
        {
            var type = typeof(T);
            IClassBase klass = null;
            if (!_classes.TryGetValue(type, out klass))
            {
                klass = AddClass(type);
            }

            return new Ref<T>(value);
        }

        private IClassBase AddClass(Type type)
        {
            return _classes[type] = new ClassBase(this, type);
        }

        private Class<T> Register<T>() where T : class, new()
        {
            return null;
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

using System;
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
            AddClass(@class.Type, @class);
            return true;
        }

        public IClassBase GetClass(string name)
        {
            return _classNames.TryGetValue(name, out var @class) ? @class : null;
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
            var @class = new Class<T>(this);
            AddClass(type, @class);
            return @class;
        }

        public object New(IClassBase @class, Stack<object> dataStack)
        {
            return @class.NewInstance(dataStack);
        }

        public IRefBase NewRef(IClassBase @class, Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public IConstRefBase NewConstRef(IClassBase @class, Stack<object> dataStack)
        {
            throw new NotImplementedException();
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
            return classBase.Create(NextId(), value);
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
            classBase.NewRef(id, out var refBase);
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
            var refBase = classBase.Create(id, value);
            _instances.Add(id, refBase);
            return refBase;
        }

        private void AddClass(Type type, IClassBase @class)
        {
            _classes[type] = @class;
            _classNames[type.Name] = @class;
        }

        private Id NextId()
        {
            return new Id(++_nextId);
        }

        private int _nextId;
        private readonly Dictionary<string, IClassBase> _classNames = new Dictionary<string, IClassBase>();
        private readonly Dictionary<Type, IClassBase> _classes = new Dictionary<Type, IClassBase>();
        private readonly Dictionary<Id, IRefBase> _instances = new Dictionary<Id, IRefBase>();
    }
}

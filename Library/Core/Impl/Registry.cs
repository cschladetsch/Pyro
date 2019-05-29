using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro.Impl
{
    /// <inheritdoc />
    /// <summary>
    /// Store of instances, and a mapping of types to facrtories.
    /// </summary>
    public class Registry
        : IRegistry
    {
        public Guid Guid { get; }

        public Registry()
        {
            BuiltinTypes.BuiltinTypes.Register(this);
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
            if (!TryGet<T>(obj, out T val))
                throw new TypeMismatchError(typeof(T), obj?.GetType());
            return val;
        }

        public bool TryGet<T>(object obj, out T val)
        {
            switch (obj)
            {
                case T value:
                    val = value;
                    return true;
                case IRefBase rb:
                    val = GetRef<T>(rb.Id).Value;
                    return true;
            }
            val = default(T);
            return false;
        }

        public IRefBase Add(object value)
        {
            var klass = GetClass(value?.GetType());
            return klass == null ? null : AddNew(klass, value);
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
            return @class.NewInstance();//dataStack);
        }

        public IRefBase NewRef(IClassBase @class, Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public IConstRefBase NewConstRef(IClassBase @class, Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public void AppendText(StringBuilder stringBuilder, object obj)
        {
            if (obj is IConstRefBase rb)
                obj = rb.BaseValue;
            var type = obj?.GetType();
            if (type == null)
                return;
            if (_classes.TryGetValue(type, out var @class))
                @class.AppendText(stringBuilder, obj);
            else
                stringBuilder.Append(obj);
        }

        public string ToText(object obj)
        {
            var str = new StringBuilder();
            AppendText(str, obj);
            return str.ToString();
        }

        public object Duplicate(object obj)
        {
            if (obj.GetType().IsValueType)
                return Activator.CreateInstance(obj.GetType(), obj);

            if (obj is IRefBase model)
                return model.Class.Duplicate(obj);

            return null;
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
            Reflect(classBase, refBase);
            AddNewInstance(classBase, refBase, id);
            return refBase as IRef<T>;
        }

        private IRef<T> AddNew<T>(IClassBase classBase, T value)
        {
            var val = AddNew<T>(classBase);
            val.Value = value;
            Reflect(classBase, val);
            return val;
        }

        private IRefBase AddNew(IClassBase classBase, object value)
        {
            var id = NextId();
            var refBase = classBase.Create(id, value);
            AddNewInstance(classBase, refBase, id);
            return refBase;
        }

        private void AddNewInstance(IClassBase classBase, IRefBase refBase, Id id)
        {
            Reflect(classBase, refBase);

            _instances.Add(id, refBase);
        }

        private void Reflect(IClassBase classBase, IRefBase refBase)
        {
            if (!(refBase.BaseValue is IReflected reflected)) 
                return;

            reflected.SelfBase = refBase;
            if (!(refBase is ConstRefBase self))
                throw new Exception("Internal error: IRef is not a ConstRef");
            self.Registry = this;
            self.Class = classBase;
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

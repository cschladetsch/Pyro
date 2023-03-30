using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Pyro.Impl {
    /// <inheritdoc />
    /// <summary>
    ///     Store of instances, and a mapping of types to facrtories.
    /// </summary>
    public class Registry
        : IRegistry {
        private readonly Dictionary<Type, IClassBase> _classes = new Dictionary<Type, IClassBase>();
        private readonly Dictionary<string, IClassBase> _classNames = new Dictionary<string, IClassBase>();

        private readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();
        private readonly Dictionary<Id, IRefBase> _instances = new Dictionary<Id, IRefBase>();

        private int _nextId;

        public Registry() {
            BuiltinTypes.BuiltinTypes.Register(this);
        }

        public Guid Guid { get; }

        public bool Register(IClassBase @class) {
            AddClass(@class.Type, @class);
            return true;
        }

        public IClassBase GetClass(string name) {
            return _classNames.TryGetValue(name, out var @class) ? @class : null;
        }

        public IRefBase GetRef(Id id) {
            return _instances[id];
        }

        public T Get<T>(object obj) {
            if (!TryGet(obj, out T val)) {
                throw new TypeMismatchError(typeof(T), obj?.GetType());
            }

            return val;
        }

        public bool TryGet<T>(object obj, out T val) {
            switch (obj) {
                case T value:
                    val = value;
                    return true;
                case IRefBase rb:
                    val = GetRef<T>(rb.Id).Value;
                    return true;
            }

            val = default;
            return false;
        }

        public IClassBase AddClass<T>(IClassBase @class) {
            throw new NotImplementedException();
        }

        public IClass<T> GetClass<T>() {
            var type = typeof(T);
            var klass = FindClass(type);
            if (klass != null) {
                return klass as IClass<T>;
            }

            var @class = new Class<T>(this);
            AddClass(type, @class);
            return @class;
        }

        public object New(IClassBase @class, Stack<object> dataStack) {
            return @class.NewInstance(); //dataStack);
        }

        public IRefBase NewRef(IClassBase @class, Stack<object> dataStack) {
            throw new NotImplementedException();
        }

        public IConstRefBase NewConstRef(IClassBase @class, Stack<object> dataStack) {
            throw new NotImplementedException();
        }

        public void ToPiScript(StringBuilder stringBuilder, object obj) {
            if (obj is IConstRefBase rb) {
                obj = rb.BaseValue;
            }

            var type = obj?.GetType();
            if (type == null) {
                return;
            }

            if (_classes.TryGetValue(type, out var @class)) {
                @class.ToPiScript(stringBuilder, obj);
            }
            else {
                stringBuilder.Append(obj);
            }
        }

        public string ToPiScript(object obj) {
            var str = new StringBuilder();
            ToPiScript(str, obj);
            return str.ToString();
        }

        public object Duplicate(object obj) {
            var type = obj.GetType();
            if (type == typeof(string)) {
                var str = (string)obj;
                var bytes = Encoding.ASCII.GetBytes(str);
                var born = Encoding.ASCII.GetString(bytes);
                Assert.IsFalse(ReferenceEquals(born, obj));
                Assert.AreEqual(obj, born);
                return born;
            }

            if (type.IsValueType) {
                var born = Convert.ChangeType(obj, type);
                Assert.AreNotSame(born, obj);
                Assert.AreEqual(obj, born);
                return born;
            }

            if (obj is IRefBase model) {
                return model.Class.Duplicate(obj);
            }

            return Activator.CreateInstance(type, obj);
        }

        public IRef<T> GetRef<T>(Id id) {
            return _instances[id].BaseValue as IRef<T>;
        }

        public IRef<T> Add<T>(T value) {
            var klass = GetClass<T>();
            if (klass == null) {
                throw new CouldNotMakeClass(value.GetType());
            }

            return AddNew(klass, value);
        }

        public IRef<T> Add<T>() {
            var klass = GetClass<T>();
            if (klass == null) {
                throw new CouldNotMakeClass(typeof(T));
            }

            return AddNew<T>(klass);
        }

        public IConstRefBase AddConst(object value) {
            var type = value?.GetType();
            var classBase = GetClass(type);
            if (classBase == null) {
                throw new CouldNotMakeClass(value?.GetType());
            }

            return classBase.Create(NextId(), value);
        }

        public IConstRef<T> AddConst<T>(T val) {
            throw new NotImplementedException();
        }

        public IConstRef<T> AddConst<T>() {
            throw new NotImplementedException();
        }

        public IClassBase GetClass(Type type) {
            if (type == null) {
                type = typeof(void);
            }

            var klass = FindClass(type);
            if (klass != null) {
                return klass;
            }

            return _classes[type] = new ClassBase(this, type);
        }

        private IRefBase Add(object value) {
            var klass = GetClass(value?.GetType());
            return klass == null ? null : AddNew(klass, value);
        }

        public object New<T>(params object[] args) {
            var klass = GetClass<T>();
            return klass.Create(NextId(), args);
        }

        public object New<T>() {
            var klass = GetClass<T>();
            if (klass == null) {
                klass = AddClass(new Class<T>(this));
            }

            return klass.NewInstance();
        }

        public IClass<T> AddClass<T>(IClass<T> klass) {
            var type = typeof(T);
            if (_classes.ContainsKey(type)) {
                throw new ClassAlreadyExists(type);
            }

            return klass;
        }

        private IClassBase FindClass(Type type) {
            return _classes.TryGetValue(type, out var klass) ? klass : null;
        }

        private IRef<T> AddNew<T>(IClassBase classBase) {
            return AddNew(classBase, (T)classBase.NewInstance());
        }

        private IRef<T> AddNew<T>(IClassBase classBase, T value) {
            var id = NextId();
            classBase.NewRef(id, out var refBase);
            refBase.BaseValue = value;
            AddNewInstance(classBase, refBase, id);
            return refBase as IRef<T>;
        }

        private IRefBase AddNew(IClassBase classBase, object value) {
            var id = NextId();
            var refBase = classBase.Create(id, value);
            AddNewInstance(classBase, refBase, id);
            return refBase;
        }

        private void AddNewInstance(IClassBase classBase, IRefBase refBase, Id id) {
            Reflect(classBase, refBase);
            _instances.Add(id, refBase);
        }

        private void Reflect(IClassBase classBase, IRefBase refBase) {
            if (!(refBase.BaseValue is IReflected reflected)) {
                return;
            }

            reflected.SelfBase = refBase;
            if (!(refBase is ConstRefBase self)) {
                throw new Exception("Internal error: IRef is not a ConstRef");
            }

            self.Registry = this;
            self.Class = classBase;
        }

        private IClassBase AddClass(Type type, IClassBase @class) {
            _classes[type] = @class;
            _classNames[type.Name] = @class;
            return @class;
        }

        private Id NextId() {
            return new Id(++_nextId);
        }
    }

    public class ClassAlreadyExists : Exception {
        public ClassAlreadyExists(Type type)
            : base($"Class already exists: {type}") {
        }
    }
}
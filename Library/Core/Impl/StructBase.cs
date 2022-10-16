using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pyro.Impl {
    /// <summary>
    /// Common for all instance types
    /// </summary>
    public class StructBase : IStructBase {
        public Type InstanceType { get; }
        public AssemblyName Assembly => _assembly;
        public Version Version => _version;
        public string TypeName => _typeName;
        public string RealTypeName => Type.FullName;
        public Type Type => _type;

        protected readonly IRegistry _registry;

        private readonly AssemblyName _assembly;
        private readonly Version _version;
        private readonly Type _type;
        private readonly string _typeName;

        protected readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        protected readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        protected readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        protected readonly Dictionary<string, EventInfo> _events = new Dictionary<string, EventInfo>();

        internal StructBase(IRegistry reg, Type type) {
            _type = type;
            _registry = reg;
            InstanceType = type;
            _typeName = type.FullName;

            foreach (var field in type.GetFields())
                _fields[field.Name] = field;

            foreach (var prop in type.GetProperties())
                _properties[prop.Name] = prop;

            foreach (var method in type.GetMethods())
                _methods[method.Name] = method;

            foreach (var ev in type.GetEvents())
                _events[ev.Name] = ev;
        }

        public void SetProperty(IRefBase obj, string name, object value) {
            if (!_properties.TryGetValue(name, out var pi))
                throw new MemberNotFoundException(TypeName, name);

            pi.SetValue(obj, value);
        }

        public object GetProperty(IRefBase obj, string name) {
            if (!_properties.TryGetValue(name, out var pi))
                throw new MemberNotFoundException(TypeName, name);

            return pi.GetValue(obj);
        }

        public void SetProperty<T>(IRefBase obj, string name, T value) {
            if (!_properties.TryGetValue(name, out var pi))
                throw new MemberNotFoundException(TypeName, name);

            pi.SetValue(obj, value);
        }

        public IRef<T> GetProperty<T>(IRefBase obj, string name) {
            // TODO: only store IRef<T> properties and fields.
            return null;
        }

        public object InvokeMethod(string name, List<object> args) {
            throw new NotImplementedException();
        }

        public void InvokeEvent(string name, List<object> args) {
            throw new NotImplementedException();
        }

        public void SetValue(IRefBase objectBase, object value) {
            throw new System.NotImplementedException();
        }
    }
}

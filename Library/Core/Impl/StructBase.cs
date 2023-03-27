using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pyro.Impl {
    /// <summary>
    /// Common for all instance types
    /// </summary>
    public class StructBase : IStructBase {
        public string TypeName { get; }

        public Type Type { get; }

        protected readonly IRegistry _registry;

        protected readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, EventInfo> _events = new Dictionary<string, EventInfo>();

        internal StructBase(IRegistry reg, Type type) {
            Type = type;
            _registry = reg;
            TypeName = type.FullName;

            foreach (var field in type.GetFields()) {
                _fields[field.Name] = field;
            }

            foreach (var prop in type.GetProperties()) {
                _properties[prop.Name] = prop;
            }

            foreach (var method in type.GetMethods()) {
                _methods[method.Name] = method;
            }

            foreach (var ev in type.GetEvents()) {
                _events[ev.Name] = ev;
            }
        }

        public void SetProperty(IRefBase obj, string name, object value) {
            if (!_properties.TryGetValue(name, out var pi)) {
                throw new MemberNotFoundException(TypeName, name);
            }

            pi.SetValue(obj, value);
        }

        public object GetProperty(IRefBase obj, string name) {
            if (!_properties.TryGetValue(name, out var pi)) {
                throw new MemberNotFoundException(TypeName, name);
            }

            return pi.GetValue(obj);
        }

        public void SetProperty<T>(IRefBase obj, string name, T value) {
            if (!_properties.TryGetValue(name, out var pi)) {
                throw new MemberNotFoundException(TypeName, name);
            }

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
    }
}

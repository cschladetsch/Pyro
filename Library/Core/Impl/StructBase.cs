using System;
using System.Collections.Generic;
using System.Reflection;

namespace Diver.Impl
{
    public class StructBase : IStructBase
    {
        public Type InstanceType { get; }
        public AssemblyName Assembly => _assembly;
        public Version Version => _version;
        public string TypeName => _typeName;

        internal StructBase(IRegistry reg, Type type)
        {
            _registry = reg;
            foreach (var prop in type.GetProperties())
                _properties[prop.Name] = prop;
            foreach (var method in type.GetMethods())
                _methods[method.Name] = method;
            foreach (var ev in type.GetEvents())
                _events[ev.Name] = ev;
        }

        public void SetProperty(IRefBase obj, string name, object value)
        {
            PropertyInfo pi = null;
            if (!_properties.TryGetValue(name, out pi))
                throw new MemberNotFoundException(TypeName, name);
            pi.SetValue(obj, value);
        }

        public object GetProperty(IRefBase obj, string name)
        {
            PropertyInfo pi = null;
            if (!_properties.TryGetValue(name, out pi))
                throw new MemberNotFoundException(TypeName, name);
            return pi.GetValue(obj);
        }

        public void SetProperty<T>(IRefBase obj, string name, T value)
        {
            PropertyInfo pi = null;
            if (!_properties.TryGetValue(name, out pi))
                throw new MemberNotFoundException(TypeName, name);
            pi.SetValue(obj, value);
            
        }

        public IRef<T> GetProperty<T>(IRefBase obj, string name)
        {
            throw new NotImplementedException();
        }

        public object InvokeMethod(string name, List<object> args)
        {
            throw new NotImplementedException();
        }

        public void InvokeEvent(string name, List<object> args)
        {
            throw new NotImplementedException();
        }

        public void SetValue(RefBase objectBase, object value)
        {
            throw new System.NotImplementedException();
        }

        private readonly AssemblyName _assembly;
        private readonly Version _version;
        private readonly string _typeName;
        private readonly IRegistry _registry;

        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, EventInfo> _events = new Dictionary<string, EventInfo>();
    }
}
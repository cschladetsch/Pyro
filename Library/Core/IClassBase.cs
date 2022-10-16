namespace Pyro {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Common interface for all Dive system classes.
    /// </summary>
    public interface IClassBase {
        AssemblyName Assembly { get; }
        Version Version { get; }
        string TypeName { get; }
        Type Type { get; }
        int TypeNumber { get; }

        object Duplicate(object obj);
        ICallable GetCallable(string name);
        void AddCallable(string name, ICallable callable);
        void SetProperty(IRefBase obj, string name, object value);
        object GetProperty(IRefBase obj, string name);
        void SetProperty<T>(IRefBase obj, string name, T value);
        IRef<T> GetProperty<T>(IRefBase obj, string name);
        object InvokeMethod(string name, List<object> args);
        void InvokeEvent(string name, List<object> args);
        void NewRef(Id id, out IRefBase refBase);
        IRefBase Create(Id id, object value);
        IConstRefBase CreateConst(Id id, object value);
        object NewInstance();
        object NewInstance(Stack<object> dataStack);
        void ToPiScript(StringBuilder str, object value);
    }
}


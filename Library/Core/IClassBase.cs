using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Diver.Impl;

namespace Diver
{
    /// <summary>
    /// Common interface for all Dive system classes.
    /// </summary>
    public interface IClassBase
    {
        AssemblyName Assembly { get; }
        Version Version { get; }
        string TypeName { get; } 
        Type Type { get; }

        ICallable GetCallable(string name);
        void AddCallable(string name, ICallable callable);
        void SetProperty(IRefBase obj, string name, object value);
        object GetProperty(IRefBase obj, string name);
    
        void SetProperty<T>(IRefBase obj, string name, T value);
        IRef<T> GetProperty<T>(IRefBase obj, string name);

        object InvokeMethod(string name, List<object> args);
        void InvokeEvent(string name, List<object> args);

        void Create(IRegistry reg, Id id, out IRefBase refBase);

        IRefBase Create(IRegistry reg, Id id, object value);
        IConstRefBase CreateConst(IRegistry reg, Id id, object value);
    }

}

//EOF


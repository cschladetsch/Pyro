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
        string TypeName { get; } 
        MethodInfo Methods { get; }
        PropertyInfo Properties { get; }
        EventInfo Events { get; }
        void SetValue(ObjectBase objectBase, object value);
    }
}

//EOF


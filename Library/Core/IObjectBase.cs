using System;

namespace Diver
{
    /// <summary>
    /// An interface to an object created by a registry.
    /// </summary>
    public interface IObjectBase
    {
        Id Id { get; }
        IRegistry Registry { get; }
        object BaseValue { get; set; }
        Type ValueType { get; }
        IClassBase Class { get; }
    }
}

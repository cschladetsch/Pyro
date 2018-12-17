using Diver.Core.Impl;

namespace Diver.Core
{
    public interface IObjectBase
    {
        Id Id { get; }
        IRegistry Registry { get; }
        object Value { get; set; }
    }
}
using System;

namespace Diver.Core
{
    public interface IRegistry
    {
        Guid Id { get; }
        IRef<T> New<T>();
        IRef<T> New<T>(T val);
    }
}

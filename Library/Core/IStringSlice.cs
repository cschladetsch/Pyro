using Pyro.Impl;

namespace Pyro
{
    /// <summary>
    /// A part of a string.
    /// </summary>
    public interface IStringSlice
    {
        IString Text { get; }
        int Start { get; }
        int End { get; }
    }
}


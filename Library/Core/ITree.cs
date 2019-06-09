using System.Collections.Generic;
using System.ComponentModel;

namespace Pyro
{
    /// <summary>
    /// A hierachy of objects with a search path.
    /// </summary>
    public interface ITree
    {
        IConstRefBase Scope { get; set; }
        IConstRefBase Parent { get; }
        IList<object> SearchPath { get; set; }

        object Resolve(IIdentifer ident);
        object Resolve(IPathname path);
    }

    public interface IIdentifer
        : ITextSerialise
    {
        bool Quoted { get; set; }
    }

    public interface IPathname
        : IIdentifer
    {
    }
}


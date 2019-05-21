using System.Collections.Generic;

namespace Pyro
{
    /// <summary>
    /// A hierachy of objects with a search path.
    /// </summary>
    public interface ITree
    {
        object Scope { get; set; }
        List<object> SearchPath { get; set; }

        object Resolve(IIdentifer ident);
        object Resolve(IPathname path);
    }

    public interface IIdentifer
        : ITextSerialise
    {
        bool Quoted { get; set; }
    }

    public interface IPathname
        : ITextSerialise
    {
    }
}

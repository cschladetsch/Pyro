using System.Collections.Generic;

namespace Pryo
{
    public interface ITree
    {
        object Scope { get; set; }
        List<object> SearchPath { get; set; }

        object Resolve(IIdentifer ident);
        object Resolve(IPathname path);
    }

    public interface IIdentifer : ITextSerialise
    {
        bool Quoted { get; set; }
    }

    public interface IPathname : ITextSerialise
    {
    }
}

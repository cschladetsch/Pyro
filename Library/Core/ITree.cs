using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver
{
    public interface ITree
    {
        object Scope { get; set; }
        List<object> SearchPath { get; set; }

        object Resolve(IIdentifer ident);
        object Resolve(IPathname path);
    }

    public interface IPathname
    {
    }
}

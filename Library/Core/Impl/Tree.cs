using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    class Tree
        : ITree
    {
        public object Scope { get; set; }
        public List<object> SearchPath { get; set; }

        public object Resolve(IIdentifer ident)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IPathname path)
        {
            throw new NotImplementedException();
        }
    }
}
